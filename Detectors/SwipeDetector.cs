using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using YouMote.States;
namespace YouMote.Detectors
{
    public enum SwipeDirection { LEFT, RIGHT, CENTER, NULL };
    abstract class SwipeDetector : ScenarioDetector
    {
        public enum SwipeZone { LEFT, RIGHT, NULL };

        /// <summary>
        /// NEED TO BE IMPLEMENTED IN SUBCLASS
        /// </summary>
        protected abstract double LEFT_SWIPE_FINISH_ANGLE { get; }
        protected abstract double RIGHT_SWIPE_FINISH_ANGLE { get; }
        protected abstract double CORRIDOR_RADIUS { get; }
        protected abstract double CORRIDOR_EPSILON { get; }
        protected abstract double LEFT_SWIPE_ZONE_ANGLE { get; }
        protected abstract double RIGHT_SWIPE_ZONE_ANGLE { get; }

        protected static double START_SHOULDER_EPSILON = 0.20;
        protected static double START_BBOX_WIDTH = 0.2;
        protected static double START_BBOX_HEIGHT = 0.2;
        protected static double START_BBOX_DEPTH = 0.2;
        protected static double STRAIGHT_ARM_MIN_ANGLE = 165;
        protected static double STRAIGHT_ARM_MAX_ANGLE = 195;
        protected static double MOVEMENT_EPSILON = 0.05;

        protected ScenarioStateHistory _history = new ScenarioStateHistory(30);


        protected SwipeDirection _direction = SwipeDirection.NULL;
        protected Skeleton _skeleton = null;
        protected Point3D _startBoxShoulderLocation = null;

        public Skeleton Skeleton
        {
            get
            {
                return _skeleton;
            }

            set
            {
                this._skeleton = value;
            }

        }

        public SwipeDetector()
        {
        }

        public Boolean isScenarioDetected()
        {
            List<ScenarioState> last3States = this._history.getLastNStates(3);
            if (last3States.Count == 3)
            {
                SwipeState state0 = (SwipeState)last3States[0];
                SwipeState state1 = (SwipeState)last3States[1];
                SwipeState state2 = (SwipeState)last3States[2];
                Boolean hasEndState = state0.Pos.Equals(SwipePosition.END);
                Boolean hasMovingState = state1.Pos.Equals(SwipePosition.MOVING);
                Boolean hasStartState = state2.Pos.Equals(SwipePosition.START);

                Boolean isScenarioDetected = hasEndState && hasMovingState && hasStartState;
                return isScenarioDetected;
            }
            else
            {
                return false;
            }
        }

        protected double getArmAngle(Point3D endpoint)
        {

            Point3D xUnitVector = new Point3D(1, 0, 0);
            Point3D shoulderLocation = this.getShoulderLocation();
            Point3D shoulderToEndpoint = shoulderLocation.subtract(endpoint);
            shoulderToEndpoint.Y = 0;
            double angle = xUnitVector.calculateAngle(shoulderToEndpoint);
            return angle;
        }
        /// <summary>
        /// returns a number -1.0 to 1.0
        /// positive number means right swipe
        /// negative number means left swipe
        /// </summary>
        /// <returns></returns>
        public double getSwipePosition()
        {
            SwipeState curState = this.getCurrentState();
            if (curState.Pos.Equals(SwipePosition.MOVING))
            {
                SwipeState prevState = this.getPreviousState();
                Point3D currentHandLocation = this.getHandLocation();
                Point3D startBoxLocation = prevState.Loc;

                double curAngle = this.getArmAngle(currentHandLocation);
                double startAngle = this.getArmAngle(startBoxLocation);
                double finishAngle = LEFT_SWIPE_FINISH_ANGLE;
                if (this._direction.Equals(SwipeDirection.RIGHT))
                {
                    finishAngle = RIGHT_SWIPE_FINISH_ANGLE;
                    if (curAngle < startAngle)
                    {
                        return 0;
                    }
                }
                else
                {
                    if (curAngle > startAngle)
                    {
                        return 0;
                    }
                }

                double percentComplete = Math.Abs(curAngle - startAngle) / Math.Abs(finishAngle - startAngle);
                return percentComplete;
            }
            else if (curState.Pos.Equals(SwipePosition.START))
            {
                return 0.0;
            }
            else if (curState.Pos.Equals(SwipePosition.END))
            {
                return 1.0;
            }
            else
            {
                return -1.0;
            }
        }

        public SwipeDirection getSwipeDirection()
        {
            return this._direction;
        }

        public SwipeState getCurrentState()
        {
            return (SwipeState)this._history.Peek();
        }

        public SwipeState getPreviousState()
        {
            if (this._history.History.Count >= 2)
            {
                return (SwipeState)this._history.getLastNStates(2).ElementAt(1);
            }
            else
            {
                return null;
            }

        }
        public void processSkeleton(Skeleton skeleton)
        {
            if (skeleton == null || this._history.History.Count == 0)
            {
                SwipeState currentState = new SwipeState(SwipePosition.NEUTRAL, DateTime.Now, DateTime.Now, new Point3D(0, 0, 0));
                this._history.addState(currentState);
                return;
            }
            this.Skeleton = skeleton;
            double armLength = this.computeArmLength();

            // determine currentState (i.e. last state on the history)
            ScenarioState ss = this._history.Peek();
            if (ss is SwipeState)
            {
                SwipeState prevState = (SwipeState)ss;

                if (prevState.End.CompareTo(DateTime.Now) >= 0)
                {
                    return;
                }
                Point3D handLocation = this.getHandLocation();

                // if curr is neutral
                // check if its straight
                if (prevState.Pos.Equals(SwipePosition.NEUTRAL))
                {
                    // determine whether arm is straight out
                    // 


                    //TODO: MAKE THIS HAPPEN
                    Boolean isArmStraight = this.isArmStraight();
                    Boolean isHandInValidStartBoxLocation = this.isHandInValidStartBoxLocation();
                    Boolean isArmShoulderHeight = this.isArmShoulderHeight();
                    if (isArmStraight && isHandInValidStartBoxLocation && isArmShoulderHeight)
                    {
                        // create a start state with position

                        SwipeZone swipeZone = this.getCurrentSwipeZone();
                        if (swipeZone.Equals(SwipeZone.LEFT))
                        {
                            this._direction = SwipeDirection.LEFT;
                        }
                        else if (swipeZone.Equals(SwipeZone.RIGHT))
                        {
                            this._direction = SwipeDirection.RIGHT;
                        }
                        else
                        {
                            this._direction = SwipeDirection.CENTER;
                        }

                        SwipeState currentState = new SwipeState(SwipePosition.START, DateTime.Now, DateTime.Now, handLocation);
                        this._history.addState(currentState);
                    }
                }
                else if (prevState.Pos.Equals(SwipePosition.START))
                {
                    // previous state was start state
                    // two things happen
                    // still in start state, but moved within box
                    // or just turned into a move state
                    Point3D startBoxLocation = prevState.Loc;
                    Boolean isHandInStartBox = this.isHandInStartBox(startBoxLocation);

                    if (isHandInStartBox)
                    {
                        // still start state
                        // just merge it
                        SwipeState currentState = new SwipeState(SwipePosition.START, DateTime.Now, DateTime.Now, handLocation);
                        this._history.addState(currentState);
                    }
                    else
                    {
                        // went outside start box

                        Boolean isValidSwipeDirection = this.isValidSwipeDirection(startBoxLocation);
                        if (isValidSwipeDirection)
                        {
                            // hand moved in valid swipe direction, keep it moving
                            this._startBoxShoulderLocation = this.getShoulderLocation();
                            SwipeState currentState = new SwipeState(SwipePosition.MOVING, DateTime.Now, DateTime.Now, handLocation);
                            this._history.addState(currentState);
                        }
                        else
                        {
                            // hand moved outside of box in wrong direction or in bad position relating to cross line
                            // make it neutral
                            SwipeState currentState = new SwipeState(SwipePosition.NEUTRAL, DateTime.Now, DateTime.Now, handLocation);
                            this._history.addState(currentState);
                        }

                    }

                }
                else if (prevState.Pos.Equals(SwipePosition.MOVING))
                {
                    List<ScenarioState> pastTwoStates = this._history.getLastNStates(2);
                    if (pastTwoStates.Count == 2)
                    {
                        ScenarioState startScenarioState = pastTwoStates[1];
                        if (startScenarioState is SwipeState)
                        {
                            SwipeState startState = (SwipeState)startScenarioState;
                            Point3D startBoxLocation = startState.Loc;
                            Point3D prevHandLocation = prevState.Loc;
                            Boolean isWithinCorridor = this.isWithinCorridor(startBoxLocation);
                            if (isWithinCorridor)
                            {
                                SwipeState currentState = new SwipeState(SwipePosition.MOVING, DateTime.Now, DateTime.Now, handLocation);
                                this._history.addState(currentState);
                            }
                            else if (this.isAfterFinishLine())
                            {
                                // add 3 seconds to make the state not destroyed by smoothing
                                SwipeState currentState = new SwipeState(SwipePosition.END, DateTime.Now, DateTime.Now.AddSeconds(1), handLocation);
                                this._history.addState(currentState);
                            }
                            else
                            {
                                // hand moved outside of box in wrong direction or in bad position relating to cross line
                                // make it neutral

                                SwipeState currentState = new SwipeState(SwipePosition.NEUTRAL, DateTime.Now, DateTime.Now, handLocation);
                                this._history.addState(currentState);
                            }
                        }
                    }

                }
                else if (prevState.Pos.Equals(SwipePosition.END))
                {
                    // prev state was end.  Now add neutral
                    SwipeState currentState = new SwipeState(SwipePosition.NEUTRAL, DateTime.Now, DateTime.Now, handLocation);
                    this._history.addState(currentState);
                }


            }


        }



        protected double computeArmLength()
        {
            Point3D shoulderPoint = this.getShoulderLocation();
            Point3D elbowPoint = this.getElbowLocation();
            Point3D handPoint = this.getHandLocation();
            double elbowToHandLength = handPoint.subtract(elbowPoint).magnitude();
            double shoulderToElbowLength = shoulderPoint.subtract(elbowPoint).magnitude();
            return elbowToHandLength + shoulderToElbowLength;
        }


        /// <summary>
        /// given a hand location, determines which zone it is in
        /// </summary>
        /// <param name="handLocation"></param>
        /// <param name="shoulderLocation"></param>
        /// <returns></returns>
        protected SwipeZone getCurrentSwipeZone()
        {
            Point3D handLocation = this.getHandLocation();
            double angle = this.getArmAngle(handLocation);
            if (angle > LEFT_SWIPE_ZONE_ANGLE)
            {
                return SwipeZone.LEFT;
            }
            else if (angle < RIGHT_SWIPE_ZONE_ANGLE)
            {
                return SwipeZone.RIGHT;
            }
            else
            {
                return SwipeZone.NULL;
            }
        }

        /// <summary>
        /// returns whether the hand is in a proper start box location (i.e. in proper zone)
        /// </summary>
        /// <param name="handLocation"></param>
        /// <returns></returns>

        protected Boolean isHandInValidStartBoxLocation()
        {

            SwipeZone currentZone = this.getCurrentSwipeZone();
            Boolean isInRightZone = currentZone.Equals(SwipeZone.RIGHT);
            Boolean isInLeftZone = currentZone.Equals(SwipeZone.LEFT);
            return isInRightZone || isInLeftZone;
        }

        /// <summary>
        /// Returns whether the arm is straight
        /// </summary>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        protected Boolean isArmStraight()
        {
            if (this._skeleton == null)
            {
                return false;
            }

            Point3D elbowRight = this.getElbowLocation();
            Point3D shoulderRight = this.getShoulderLocation();
            Point3D handRight = this.getHandLocation();

            Point3D elbowToShoulder = elbowRight.subtract(shoulderRight);
            Point3D elbowToHand = elbowRight.subtract(handRight);

            double armAngle = elbowToShoulder.calculateAngle(elbowToHand);
            Boolean isMinAngle = armAngle > STRAIGHT_ARM_MIN_ANGLE;
            Boolean isMaxAngle = armAngle < STRAIGHT_ARM_MAX_ANGLE;
            return isMinAngle && isMaxAngle;
        }


        /// <summary>
        /// returns whether the hand is going in the correct direction and if it is an appropriate in relation to cross line
        /// </summary>
        /// <param name="handLocation"></param>
        /// <param name="startBoxLocation"></param>
        /// <returns></returns>
        protected Boolean isValidSwipeDirection(Point3D startBoxLocation)
        {

            Point3D handLocation = this.getHandLocation();
            Point3D shoulderLocation = this.getShoulderLocation();
            // called when was in a start state, but now has left the startBox.
            Point3D locationChange = handLocation.subtract(startBoxLocation);
            Boolean withinY = Math.Abs(locationChange.Y) < START_BBOX_HEIGHT;
            Boolean withinZ = Math.Abs(locationChange.Z) < START_BBOX_DEPTH;

            Boolean correctXDirection = false;
            if (this._direction.Equals(SwipeDirection.LEFT) && locationChange.X <= 0)
            {
                correctXDirection = true;
            }
            else if (this._direction.Equals(SwipeDirection.RIGHT) && locationChange.X >= 0)
            {
                correctXDirection = true;
            }

            return withinY && withinZ && correctXDirection;
        }

        protected Boolean isArmShoulderHeight()
        {
            Point3D handLocation = this.getHandLocation();
            Point3D shoulderLocation = this.getShoulderLocation();
            double yDiff = Math.Abs(handLocation.Y - shoulderLocation.Y);
            if (yDiff < START_SHOULDER_EPSILON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// returns true if the hand is still in the startBox Location
        /// </summary>
        /// <param name="handLocation"></param>
        /// <returns></returns>
        protected Boolean isHandInStartBox(Point3D startBoxLocation)
        {
            Point3D handLocation = this.getHandLocation();
            Boolean withinX = Math.Abs(handLocation.X - startBoxLocation.X) < START_BBOX_WIDTH / 2.0;
            Boolean withinY = Math.Abs(handLocation.Y - startBoxLocation.Y) < START_BBOX_HEIGHT / 2.0;
            Boolean withinZ = Math.Abs(handLocation.Z - startBoxLocation.Z) < START_BBOX_DEPTH / 2.0;
            return withinX && withinY && withinZ;
        }

        /// <summary>
        /// Returns the Point3D for the hand location.
        /// </summary>
        /// <returns></returns>
        abstract protected Point3D getHandLocation();


        /// <summary>
        /// Returns the Point3D for the elbow location.
        /// </summary>
        /// <returns></returns>
        abstract protected Point3D getElbowLocation();

        /// <summary>
        /// Returns the Point3 D for the shoulder location.
        /// </summary>
        /// <returns></returns>
        abstract protected Point3D getShoulderLocation();

        protected Boolean isBeforeStartLine()
        {
            return this.getSwipePosition() < 0;
        }
        protected Boolean isAfterFinishLine()
        {
            return this.getSwipePosition() > 1;

        }

        protected Boolean isWithinCorridor(Point3D startBoxLocation)
        {
            Point3D handLocation = this.getHandLocation();
            Point3D startBoxShoulderLocation = this._startBoxShoulderLocation;

            double startArmLength = startBoxShoulderLocation.subtract(startBoxLocation).magnitude();
            double currentArmLength = startBoxShoulderLocation.subtract(handLocation).magnitude();

            double difference = Math.Abs(startArmLength - currentArmLength);
            Boolean isWithinCorridorEpsilon = difference < CORRIDOR_EPSILON;

            Boolean isWithinCorridorRadius = Math.Abs(handLocation.Y - startBoxLocation.Y) < CORRIDOR_RADIUS;
            Boolean isAfterStartLine = !this.isBeforeStartLine();
            Boolean isBeforeFinishLine = !this.isAfterFinishLine();
            return isWithinCorridorEpsilon && isWithinCorridorRadius && isAfterStartLine && isBeforeFinishLine;
        }

    }
}
