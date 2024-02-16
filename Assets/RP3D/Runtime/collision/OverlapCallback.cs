using System.Collections.Generic;

namespace RP3D
{
    public abstract class OverlapCallback
    {
        /// Enumeration EventType that describes the type of overlapping event
        public enum EventType
        {
            /// This overlap is a new overlap between the two
            /// colliders (the colliders where not overlapping in the previous frame)
            OverlapStart,

            /// The two colliders were already overlapping in the previous frame and this is a new or updated overlap
            OverlapStay,

            /// The two colliders were overlapping in the previous frame and are not overlapping anymore
            OverlapExit
        }

        private ContactPair mContactPair;
        private bool mIsLostOverlapPair;
        private PhysicsWorld mWorld;

        /// Return a pointer to the first collider in contact
        public Collider getCollider1()
        {
            return mWorld.mCollidersComponents.getCollider(mContactPair.Collider1Entity);
        }

        /// Return a pointer to the second collider in contact
        public Collider getCollider2()
        {
            return mWorld.mCollidersComponents.getCollider(mContactPair.Collider2Entity);
        }

        /// Return a pointer to the first body in contact
        public CollisionBody getBody1()
        {
            return mWorld.mCollisionBodyComponents.getBody(mContactPair.Body1Entity);
        }

        /// Return a pointer to the second body in contact
        public CollisionBody getBody2()
        {
            return mWorld.mCollisionBodyComponents.getBody(mContactPair.Body2Entity);
        }

        /// Return the corresponding type of event for this overlapping pair
        public EventType getEventType()
        {
            if (mIsLostOverlapPair) return EventType.OverlapExit;
            if (mContactPair.CollidingInPreviousFrame) return EventType.OverlapStay;
            return EventType.OverlapStart;
        }

        public abstract void onOverlap(CallbackData callbackData);

        // Class OverlapPair
        /**
         * This class represents the contact between two colliders of the physics world.
         */
        public class OverlapPair
        {
            /// Enumeration EventType that describes the type of overlapping event
            public enum EventType
            {
                /// This overlap is a new overlap between the two
                /// colliders (the colliders where not overlapping in the previous frame)
                OverlapStart,

                /// The two colliders were already overlapping in the previous frame and this is a new or updated overlap
                OverlapStay,

                /// The two colliders were overlapping in the previous frame and are not overlapping anymore
                OverlapExit
            }


            // -------------------- Attributes -------------------- //

            /// Contact pair
            private readonly ContactPair mContactPair;

            /// True if the pair were overlapping in the previous frame but not in the current one
            private readonly bool mIsLostOverlapPair;

            /// Reference to the physics world
            private readonly PhysicsWorld mWorld;

            // -------------------- Methods -------------------- //

            /// Constructor
            public OverlapPair(ContactPair contactPair, PhysicsWorld world, bool isLostOverlappingPair)
            {
                mContactPair = contactPair;
                mWorld = world;
                mIsLostOverlapPair = isLostOverlappingPair;
            }


            // -------------------- Methods -------------------- //


            /// Return a pointer to the first collider in contact
            public Collider getCollider1()
            {
                return mWorld.mCollidersComponents.getCollider(mContactPair.Collider1Entity);
            }

            /// Return a pointer to the second collider in contact
            public Collider getCollider2()
            {
                return mWorld.mCollidersComponents.getCollider(mContactPair.Collider2Entity);
            }

            /// Return a pointer to the first body in contact
            public CollisionBody getBody1()
            {
                return mWorld.mCollisionBodyComponents.getBody(mContactPair.Body1Entity);
            }

            /// Return a pointer to the second body in contact
            public CollisionBody getBody2()
            {
                return mWorld.mCollisionBodyComponents.getBody(mContactPair.Body2Entity);
            }

            /// Return the corresponding type of event for this overlapping pair
            public EventType getEventType()
            {
                if (mIsLostOverlapPair) return EventType.OverlapExit;

                if (mContactPair.CollidingInPreviousFrame) return EventType.OverlapStay;

                return EventType.OverlapStart;
            }
        }

        // Class CallbackData
        /**
         * This class contains data about overlap between bodies
         */
        public class CallbackData
        {
            // -------------------- Attributes -------------------- //

            /// Reference to the array of contact pairs (contains contacts and triggers events)
            private readonly List<ContactPair> mContactPairs;

            /// Array of indices of the mContactPairs array that are overlap/triggers events (not contact events)
            private readonly List<int> mContactPairsIndices;

            /// Reference to the array of lost contact pairs (contains contacts and triggers events)
            private readonly List<ContactPair> mLostContactPairs;

            /// Array of indices of the mLostContactPairs array that are overlap/triggers events (not contact events)
            private readonly List<int> mLostContactPairsIndices;

            /// Reference to the physics world
            private readonly PhysicsWorld mWorld;

            // -------------------- Methods -------------------- //

            /// Constructor
            // CollisionCallbackData Constructor
            public CallbackData(List<ContactPair> contactPairs, List<ContactPair> lostContactPairs,
                bool onlyReportTriggers, PhysicsWorld world)
            {
                mContactPairs = contactPairs;
                mLostContactPairs = lostContactPairs;
                mContactPairsIndices = new List<int>();
                mLostContactPairsIndices = new List<int>();
                mWorld = world;

                // Filter the contact pairs to only keep the overlap/trigger events (not the contact events)
                var nbContactPairs = mContactPairs.Count;
                for (var i = 0; i < nbContactPairs; i++)
                    // If the contact pair contains contacts (and is therefore not an overlap/trigger event)
                    if (!onlyReportTriggers || mContactPairs[i].IsTrigger)
                        mContactPairsIndices.Add(i);
                // Filter the lost contact pairs to only keep the overlap/trigger events (not the contact events)
                var nbLostContactPairs = mLostContactPairs.Count;
                for (var i = 0; i < nbLostContactPairs; i++)
                    // If the contact pair contains contacts (and is therefore not an overlap/trigger event)
                    if (!onlyReportTriggers || mLostContactPairs[i].IsTrigger)
                        mLostContactPairsIndices.Add(i);
            }


            // -------------------- Methods -------------------- //

            /// Return the number of overlapping pairs of bodies
            public int getNbOverlappingPairs()
            {
                return mContactPairsIndices.Count + mLostContactPairsIndices.Count;
            }

            /// Return a given overlapping pair of bodies
            public OverlapPair getOverlappingPair(int index)
            {
                if (index < mContactPairsIndices.Count)
                    // Return a contact pair
                    return new OverlapPair(mContactPairs[mContactPairsIndices[index]], mWorld, false);
                // Return a lost contact pair
                return new OverlapPair(mLostContactPairs[mLostContactPairsIndices[index - mContactPairsIndices.Count]],
                    mWorld, true);
            }
        }
    }
}