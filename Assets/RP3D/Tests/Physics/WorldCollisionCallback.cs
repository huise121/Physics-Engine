using System.Collections.Generic;
using RP3D;

namespace Scenes.Test
{
    // Contact point collision data
    public class CollisionPointData
    {
        private readonly PVector3 localPointBody1;
        private readonly PVector3 localPointBody2;
        private readonly float penetrationDepth;

        public CollisionPointData(PVector3 point1, PVector3 point2, float penDepth)
        {
            localPointBody1 = point1;
            localPointBody2 = point2;
            penetrationDepth = penDepth;
        }

        public bool isContactPointSimilarTo(PVector3 pointBody1, PVector3 pointBody2, float penDepth,
            float epsilon = 0.001f)
        {
            return PMath.approxEqual(pointBody1, localPointBody1, epsilon) &&
                   PMath.approxEqual(pointBody2, localPointBody2, epsilon) &&
                   PMath.approxEqual(penetrationDepth, penDepth, epsilon);}
        }
        

    // Contact pair collision data
    public class ContactPairData
    {
        public List<CollisionPointData> contactPoints;

        public ContactPairData()
        {
            contactPoints = new List<CollisionPointData>();
        }
        
        public int getNbContactPoints()
        {
            return contactPoints.Count;
        }

        public bool hasContactPointSimilarTo(PVector3 localPointBody1, PVector3 localPointBody2, float penetrationDepth,
            float epsilon = 0.001f)
        {
            foreach (var pointData in contactPoints)
                if (pointData.isContactPointSimilarTo(localPointBody1, localPointBody2, penetrationDepth, epsilon))
                    return true;
            return false;
        }
    }

    // Collision data between two colliders
    public class CollisionData
    {
        public Pair<Collider, Collider> colliders;
        public Pair<CollisionBody, CollisionBody> bodies;
        public List<ContactPairData> contactPairs;

        public CollisionData()
        {
            contactPairs = new List<ContactPairData>();
        }

        public int getNbContactPairs()
        {
            return contactPairs.Count;
        }

        public int getTotalNbContactPoints()
        {
            var nbPoints = 0;
            foreach (var pairData in contactPairs) nbPoints += pairData.getNbContactPoints();
            return nbPoints;
        }

        public CollisionBody getBody1()
        {
            return bodies.First;
        }

        public CollisionBody getBody2()
        {
            return bodies.Second;
        }

        public bool hasContactPointSimilarTo(PVector3 localPointBody1, PVector3 localPointBody2, float penetrationDepth,
            float epsilon = 0.001f)
        {
            foreach (var pairData in contactPairs)
                if (pairData.hasContactPointSimilarTo(localPointBody1, localPointBody2, penetrationDepth, epsilon))
                    return true;
            return false;
        }
    }

    // Class
    public class WorldCollisionCallback : CollisionCallback
    {
        private Dictionary<Pair<Collider, Collider>, CollisionData> mCollisionDatas;

        private Pair<Collider, Collider> getCollisionKeyPair(Pair<Collider, Collider> pair)
        {
            if (pair.First.GetEntity().Id > pair.Second.GetEntity().Id)
            {
                return new Pair<Collider, Collider>(pair.Second, pair.First);
            }
            return pair;
        }


        public WorldCollisionCallback()
        {
            mCollisionDatas = new Dictionary<Pair<Collider, Collider>, CollisionData>();
            reset();
        }

        public void reset()
        {
            mCollisionDatas.Clear();
        }

        public bool hasContacts()
        {
            return mCollisionDatas.Count > 0;
        }


        public bool areCollidersColliding(Collider collider1, Collider collider2)
        {
            Pair<Collider, Collider> keyPair;
            if (collider1.GetEntity().Id>collider2.GetEntity().Id)
                keyPair = new Pair<Collider, Collider>(collider2, collider1);
            else
            {
                keyPair = new Pair<Collider, Collider>(collider1, collider2);
            }
            return mCollisionDatas.ContainsKey(keyPair);
        }


        public CollisionData getCollisionData(Collider collider1, Collider collider2)
        {
            Pair<Collider, Collider> keyPair;
            if (collider1.GetEntity().Id>collider2.GetEntity().Id)
                keyPair = new Pair<Collider, Collider>(collider2, collider1);
            else
            {
                keyPair = new Pair<Collider, Collider>(collider1, collider2);
            }
            if (mCollisionDatas.TryGetValue(keyPair, out var collisionData))
                return collisionData;
            return null;
        }

        // This method is called when some contacts occur
        public override void onContact(CallbackData callbackData)
        {
            var collisionData = new CollisionData();

            // For each contact pair
            for (var p = 0; p < callbackData.GetNbContactPairs(); p++)
            {
                ContactPairData contactPairData = new ContactPairData();
                CollisionCallback.ContactPair contactPair = callbackData.GetContactPair(p);

                collisionData.bodies =
                    new Pair<CollisionBody, CollisionBody>(contactPair.GetBody1(), contactPair.GetBody2());
                collisionData.colliders = new Pair<Collider, Collider>(contactPair.GetCollider1(), contactPair.GetCollider2());

                // For each contact point
                for (var c = 0; c < contactPair.GetNbContactPoints(); c++)
                {
                    CollisionCallback.ContactPoint contactPoint = contactPair.GetContactPoint(c);
                    CollisionPointData collisionPoint = new CollisionPointData(contactPoint.GetLocalPointOnCollider1(),
                        contactPoint.GetLocalPointOnCollider2(), contactPoint.GetPenetrationDepth());
                    contactPairData.contactPoints.Add(collisionPoint);
                }

                collisionData.contactPairs.Add(contactPairData);
            }
            mCollisionDatas.Add(getCollisionKeyPair(collisionData.colliders),collisionData);
        }
    }

    /// Overlap callback
    public class WorldOverlapCallback : OverlapCallback
    {
        private List<Pair<CollisionBody, CollisionBody>> mOverlapBodies;

        public WorldOverlapCallback()
        {
            mOverlapBodies = new List<Pair<CollisionBody, CollisionBody>>();
            reset();
        }

        /// This method will be called for each reported overlapping bodies
        public override void onOverlap(CallbackData callbackData)
        {
            // For each overlapping pair
            for (var i = 0; i < callbackData.getNbOverlappingPairs(); i++)
            {
                var overlapPair = callbackData.getOverlappingPair(i);
                mOverlapBodies.Add(
                    new Pair<CollisionBody, CollisionBody>(overlapPair.getBody1(), overlapPair.getBody2()));
            }
        }

        public void reset()
        {
            mOverlapBodies.Clear();
        }

        public bool hasOverlapWithBody(CollisionBody collisionBody)
        {
            for (var i = 0; i < mOverlapBodies.Count; i++)
                if (mOverlapBodies[i].First == collisionBody || mOverlapBodies[i].Second == collisionBody)
                    return true;
            return false;
        }
    }


}