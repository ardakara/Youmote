using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using YouMote.States;

namespace YouMote.Detectors
{


    abstract class VolumeSwipeDetector : ScenarioDetector
    {

        public enum VolumeSwipeDirection { UP, DOWN, CENTER, NULL };

        /// <summary>
        /// NEED TO BE IMPLEMENTED IN SUBCLASS
        /// </summary>
        protected static Point3D X_UNIT_VECTOR = new Point3D(1, 0, 0);
        protected static Point3D Y_UNIT_VECTOR = new Point3D(0, 1, 0);
        protected static Point3D Z_UNIT_VECTOR = new Point3D(0, 0, 1);
        protected static double SWIPE_FINISH_ANGLE = 20;

        protected abstract double LEFT_SWIPE_ZONE_ANGLE { get; }
        protected abstract double RIGHT_SWIPE_ZONE_ANGLE { get; }

        protected static double CORRIDOR_RADIUS = 0.25;
        protected static double CORRIDOR_EPSILON = 0.25;

        protected static double START_SHOULDER_EPSILON = 0.20;
        protected static double START_BBOX_WIDTH = 0.1;
        protected static double START_BBOX_HEIGHT = 0.1;
        protected static double START_BBOX_DEPTH = 0.1;
        protected static double STRAIGHT_ARM_MIN_ANGLE = 165;
        protected static double STRAIGHT_ARM_MAX_ANGLE = 195;
        protected static double MOVEMENT_EPSILON = 0.05;

        protected ScenarioStateHistory _history = new ScenarioStateHistory(30);


        protected VolumeSwipeDirection _direction = VolumeSwipeDirection.NULL;
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

        public VolumeSwipeDetector()
        {
        }

        public Boolean isScenarioDetected()
        {
            List<ScenarioState> last3States = this._history.getLastNStates(3);
            if (last3States.Count >= 3)
            {
                SwipeState state0 = (SwipeState)last3States[0];
                SwipeState state1 = (SwipeState)last3States[1];

                Boolean hasMovingState0 = state0.Pos.Equals(SwipePosition.MOVING);
                Boolean hasStartState1 = state1.Pos.Equals(SwipePosition.START);

                Boolean hasEndState0 = state0.Pos.Equals(SwipePosition.END);
                Boolean hasMovingState1 = state1.Pos.Equals(SwipePosition.MOVING);
                Boolean isScenarioDetected = false;
                if (last3States.Count == 3)
                {
                    SwipeState state2 = (SwipeState)last3States[2];
                    Boolean hasStartState2 = state2.Pos.Equals(SwipePosition.START);
                    isScenarioDetected = isScenarioDetected || (hasEndState0 && hasMovingState1 && hasStartState2);
                }

                isScenarioDetected = isScenarioDetected || (hasMovingState0 && hasStartState1);
                return isScenarioDetected;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Computes the angle of your shoulder to enpoint flattened onto the unit-vector axis
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="unitVector"></param>
        /// <returns></returns>
        protected double computeArmAngle(Point3D endpoint, Point3D unitVector)
        {
            Point3D shoulderLocation = this.getShoulderLocation();
            Point3D shoulderToEndpoint = shoulderLocation.subtract(endpoint);
            double angle = unitVector.calculateAngle(shoulderToEndpoint);
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
            if (curState.Pos.Equals(SwipePosition.MOVING)||curState.Pos.Equals(SwipePosition.END))
            {

                if (this._direction.Equals(VolumeSwipeDirection.CENTER) || this._direction.Equals(VolumeSwipeDirection.NULL))
                {
                    return 0;
                }

                Point3D currentHandLocation = this.getHandLocation();
                Point3D startBoxLocation = this.getStartBoxLocation();

                double curAngle = this.computeArmAngle(currentHandLocation, Y_UNIT_VECTOR);
                double startAngle = this.computeArmAngle(startBoxLocation, Y_UNIT_VECTOR);
                double finishAngle = startAngle + (this._direction.Equals(VolumeSwipeDirection.UP) ? -1 : 1) * SWIPE_FINISH_ANGLE;
                double percentComplete = Math.Abs(curAngle - startAngle) / Math.Abs(finishAngle - startAngle);
                if (percentComplete > 1)
                {
                    percentComplete = 1;
                }
                return percentComplete;
            }
            else if (curState.Pos.Equals(SwipePosition.START))
            {
                return 0.0;
            }
            else
            {
                return -1.0;
            }
        }

        public VolumeSwipeDirection getSwipeDirection()
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
                        this._direction = VolumeSwipeDirection.CENTER;
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
                    Boolean isHandInStartBox = this.isHandInStartBox();

                    if (isHandInStartBox)
                    {
                        // still start state
                        // just merge it
                        this._direction = VolumeSwipeDirection.CENTER;
                        SwipeState currentState = new SwipeState(SwipePosition.START, DateTime.Now, DateTime.Now, handLocation);
                        this._history.addState(currentState);
                    }
                    else
                    {
                        // went outside start box

                        Boolean isValidSwipeDirection = this.isValidSwipeDirection();
                        if (isValidSwipeDirection)
                        {
                            // hand moved in valid swipe direction, keep it moving
                            this._startBoxShoulderLocation = this.getShoulderLocation();
                            this._direction = this.computeCurrentSwipeDirection();

                            SwipeState currentState = new SwipeState(SwipePosition.MOVING, DateTime.Now, DateTime.Now, handLocation);
                            this._history.addState(currentState);
                        }
                        else
                        {
                            this._direction = VolumeSwipeDirection.NULL;
                            // hand moved outside of box in wrong direction or in bad position relating to cross line
                            // make it neutral
                            SwipeState currentState = new SwipeState(SwipePosition.NEUTRAL, DateTime.Now, DateTime.Now, handLocation);
                            this._history.addState(currentState);
                        }

                    }

                }
                else if (prevState.Pos.Equals(SwipePosition.MOVING))
                {
                    Boolean isWithinCorridor = this.isWithinCorridor();
                    Boolean isAfterFinishLine = this.isAfterFinishLine();
                    if (isWithinCorridor && isAfterFinishLine)
                    {
                        // add 3 seconds to make the state not destroyed by smoothing
                        this._direction = this.computeCurrentSwipeDirection();
                        SwipeState currentState = new SwipeState(SwipePosition.END, DateTime.Now, DateTime.Now, handLocation);
                        this._history.addState(currentState);
                    }
                    else if (isWithinCorridor)
                    {
                        this._direction = this.computeCurrentSwipeDirection();
                        SwipeState currentState = new SwipeState(SwipePosition.MOVING, DateTime.Now, DateTime.Now, handLocation);
                        this._history.addState(currentState);
                    }
                    else
                    {


                    Boolean isWithinCorridor2 = this.isWithinCorridor();
                    Boolean isAfterFinishLine2 = this.isAfterFinishLine();
                        // hand moved outside of box in wrong direction or in bad position relating to cross line
                        // make it neutral
                        this._direction = VolumeSwipeDirection.NULL;
                        SwipeState currentState = new SwipeState(SwipePosition.NEUTRAL, DateTime.Now, DateTime.Now, handLocation);
                        this._history.addState(currentState);
                    }
                }
                else if (prevState.Pos.Equals(SwipePosition.END))
                {
                    Boolean isWithinCorridor = this.isWithinCorridor();
                    Boolean isAfterFinishLine = this.isAfterFinishLine();
                    if (isAfterFinishLine)
                    {
                        this._direction = this.computeCurrentSwipeDirection();
                        SwipeState currentState = new SwipeState(SwipePosition.END, DateTime.Now, DateTime.Now, handLocation);
                        this._history.addState(currentState);
                    }
                    else if (isWithinCorridor && !isAfterFinishLine)
                    {
                        this._direction = this.computeCurrentSwipeDirection();
                        SwipeState currentState = new SwipeState(SwipePosition.MOVING, DateTime.Now, DateTime.Now, handLocation);
                        this._history.addState(currentState);
                    }
                    else
                    {
                        this._direction = VolumeSwipeDirection.NULL;
                        // prev state was end.  Now add neutral
                        SwipeState currentState = new SwipeState(SwipePosition.NEUTRAL, DateTime.Now, DateTime.Now, handLocation);
                        this._history.addState(currentState);
                    }
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
        /// returns whether the hand is in a proper start box location (i.e. in proper zone)
        /// </summary>
        /// <param name="handLocation"></param>
        /// <returns></returns>

        protected Boolean isHandInValidStartBoxLocation()
        {
            Point3D handLocation = this.getHandLocation();
            double angle = this.computeArmAngle(handLocation, X_UNIT_VECTOR);

            if (angle < LEFT_SWIPE_ZONE_ANGLE && angle > RIGHT_SWIPE_ZONE_ANGLE)
            {
                return true;
            }
            else
            {
                return false;
            }
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

        protected Point3D getStartBoxLocation()
        {
            if (this.getCurrentState().Pos.Equals(SwipePosition.NEUTRAL))
            {
                return null;
            }
            else if (this.getCurrentState().Pos.Equals(SwipePosition.START))
            {
                return this.getCurrentState().Loc;
            }
            else if (this.getCurrentState().Pos.Equals(SwipePosition.MOVING))
            {
                if (this._history.History.Count > 1)
                {
                    SwipeState ss = (SwipeState)this._history.getLastNStates(2).ElementAt(1);
                    return ss.Loc;
                }
                else
                {
                    return null;
                }

            }

            else if (this.getCurrentState().Pos.Equals(SwipePosition.END))
            {

                if (this._history.History.Count > 1)
                {
                    SwipeState ss = (SwipeState)this._history.getLastNStates(3).ElementAt(2);
                    return ss.Loc;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// returns whether the hand is going in the correct direction and if it is an appropriate in relation to cross line
        /// </summary>
        /// <param name="handLocation"></param>
        /// <param name="startBoxLocation"></param>
        /// <returns></returns>
        protected Boolean isValidSwipeDirection()
        {
            Point3D startBoxLocation = this.getStartBoxLocation();

            Point3D handLocation = this.getHandLocation();
            Point3D shoulderLocation = this.getShoulderLocation();
            // called when was in a start state, but now has left the startBox.
            Point3D locationChange = handLocation.subtract(startBoxLocation);
            Boolean withinX = Math.Abs(locationChange.X) < START_BBOX_WIDTH;
            Boolean withinZ = Math.Abs(locationChange.Z) < START_BBOX_DEPTH;
            return withinX && withinZ;
        }

        protected VolumeSwipeDirection computeCurrentSwipeDirection()
        {
            Point3D startBoxLocation = this.getStartBoxLocation();
            Point3D handLocation = this.getHandLocation();
            Point3D shoulderLocation = this.getShoulderLocation();
            // called when was in a start state, but now has left the startBox.
            Point3D locationChange = handLocation.subtract(startBoxLocation);
            return locationChange.Y > 0 ? VolumeSwipeDirection.UP : VolumeSwipeDirection.DOWN;
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
        protected Boolean isHandInStartBox()
        {
            Point3D startBoxLocation = this.getStartBoxLocation();
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

        protected Boolean isAfterFinishLine()
        {
            return this.getSwipePosition() >= 1;

        }

        protected Boolean isWithinCorridor()
        {
            Point3D startBoxLocation = this.getStartBoxLocation();
            Point3D handLocation = this.getHandLocation();
            Point3D startBoxShoulderLocation = this._startBoxShoulderLocation;

            double startArmLength = startBoxShoulderLocation.subtract(startBoxLocation).magnitude();
            double currentArmLength = startBoxShoulderLocation.subtract(handLocation).magnitude();

            double difference = Math.Abs(startArmLength - currentArmLength);
            Boolean isWithinCorridorEpsilon = difference < CORRIDOR_EPSILON;

            Boolean isWithinCorridorRadius = Math.Abs(handLocation.X - startBoxLocation.X) < CORRIDOR_RADIUS;
            return isWithinCorridorEpsilon && isWithinCorridorRadius;
        }

    }
}
