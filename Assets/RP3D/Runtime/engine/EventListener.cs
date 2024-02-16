
using UnityEngine;

namespace RP3D
{
    public  class EventListener:CollisionCallback
    {
        public override void onContact(CallbackData callbackData)
        {

            for (int p=0; p < callbackData.GetNbContactPairs(); p++) {

                ContactPair contactPair = callbackData.GetContactPair(p);

                // For each contact point of the contact pair
                for (int c=0; c < contactPair.GetNbContactPoints(); c++) {

                    ContactPoint contactPoint = contactPair.GetContactPoint(c);

                    // Contact normal
                    PVector3 normal = contactPoint.GetWorldNormal();
                    PVector3 contactNormal = new PVector3(normal.x, normal.y, normal.z);

                    PVector3 point1 = contactPoint.GetLocalPointOnCollider1();
                    point1 = contactPair.GetCollider1().GetLocalToWorldTransform() * point1;

                    PVector3 position1 = new PVector3(point1.x, point1.y, point1.z);
                   // mContactPoints.Add(SceneContactPoint(position1, contactNormal, openglframework::Color::red()));

                    PVector3 point2 = contactPoint.GetLocalPointOnCollider2();
                    point2 = contactPair.GetCollider2().GetLocalToWorldTransform() * point2;
                    PVector3 position2 = new PVector3(point2.x, point2.y, point2.z);
                   // mContactPoints.Add(SceneContactPoint(position2, contactNormal, openglframework::Color::blue()));
                }
            }
            
            //Debug.Log("onContact:");
        }

        public void onTrigger(OverlapCallback.CallbackData callbackData)
        {
            //Debug.Log("onTrigger:");
        }
    }
}