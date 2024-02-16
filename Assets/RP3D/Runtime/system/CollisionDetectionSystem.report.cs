using System.Collections.Generic;

namespace RP3D
{
    public partial class CollisionDetectionSystem
    {
        // 报告接触和触发器
        public void reportContactsAndTriggers()
        {
            // Report contacts and triggers to the user
            if (mWorld.mEventListener != null)
            {
                reportContacts(mWorld.mEventListener, mCurrentContactPairs, mCurrentContactManifolds,
                    mCurrentContactPoints, mLostContactPairs);
                reportTriggers(mWorld.mEventListener, mCurrentContactPairs, mLostContactPairs);
            }

            // Report contacts for debug rendering (if enabled)
            // if (mWorld.mIsDebugRenderingEnabled)
            //     reportDebugRenderingContacts(mCurrentContactPairs, mCurrentContactManifolds, mCurrentContactPoints,
            //         mLostContactPairs);

            mOverlappingPairs.updateCollidingInPreviousFrame();

            mLostContactPairs.Clear();
        }
        
        // 向用户报告所有接触
        private void reportContacts(CollisionCallback callback, List<ContactPair> contactPairs,
            List<ContactManifold> manifolds, List<ContactPoint> contactPoints, List<ContactPair> lostContactPairs)
        {
            //If there are contacts
            if (contactPairs.Count + lostContactPairs.Count > 0)
            {
                var callbackData = new CollisionCallback.CallbackData(contactPairs, manifolds, contactPoints,
                    lostContactPairs, mWorld);
                // Call the callback method to report the contacts
                callback.onContact(callbackData);
            }
        }

        // 向用户报告所有触发器
        private void reportTriggers(EventListener eventListener, List<ContactPair> contactPairs,
            List<ContactPair> lostContactPairs)
        {
            // If there are contacts
            if (contactPairs.Count + lostContactPairs.Count > 0)
            {
                OverlapCallback.CallbackData callbackData = new OverlapCallback.CallbackData(contactPairs, lostContactPairs, true, mWorld);

                // Call the callback method to report the overlapping shapes
                eventListener.onTrigger(callbackData);
            }
        }
        
        
        
        // Report all contacts for debug rendering
        // public void reportDebugRenderingContacts(List<ContactPair> contactPairs, List<ContactManifold> manifolds,
        //     List<ContactPoint> contactPoints, List<ContactPair> lostContactPairs)
        // {
        //     // If there are contacts
        //     if (contactPairs.Count + lostContactPairs.Count > 0)
        //     {
        //         {
        //             CallbackData callbackData =
        //                 new CallbackData(contactPairs, manifolds, contactPoints, lostContactPairs, mWorld);
        //
        //             // Call the callback method to report the contacts
        //             mWorld.mDebugRenderer.onContact(callbackData);
        //         }
        //     }
        // }
        
        
    }
}