
//ContactPoint（接触点）通常表示物体之间接触的点。
//在物理引擎中，当两个物体发生碰撞或接触时，它们会在接触面上产生一个或多个接触点。
//每个接触点通常包含了一些关键信息，如接触位置、法线方向、接触深度等。

//接触点是物理引擎用于计算碰撞响应的重要数据。通过接触点，可以计算出碰撞力、摩擦力等影响物体运动的参数。
//物理引擎会根据接触点的信息来模拟物体之间的碰撞效果，从而实现逼真的物理模拟。

//接触点的信息通常在碰撞检测过程中被计算得到，并且在碰撞解析（collision resolution）阶段被使用。
//接触点的数量和位置可以根据物体之间的碰撞形状和相互作用来动态变化。

using System.Runtime.CompilerServices;

namespace RP3D
{
    public class ContactPoint
    {
        /// <summary>
        /// 在 ContactPoint 中，mNormal 通常表示接触点的法线方向。
        /// 法线方向是指与碰撞表面垂直的方向，它指示了碰撞表面的朝向或者碰撞的方向。
        ///
        /// 在物理引擎中，法线方向是计算碰撞响应所必需的关键信息。
        /// 它被用于计算碰撞力、摩擦力等物理效应，并且也可以用于确定物体之间的相对运动方向。
        ///
        /// mNormal 可能是一个三维向量，表示接触点的法线方向。
        /// 这个向量通常是单位向量，表示了接触表面在接触点处的法线方向。
        /// 根据具体的物理引擎实现，mNormal 的定义和用法可能会有所不同，但通常用于描述接触点的法线信息。
        /// </summary>
        private PVector3 mNormal;

        /// <summary>
        /// 在 ContactPoint 中，mPenetrationDepth 通常表示接触点的侵入深度（penetration depth）。
        /// 侵入深度是指两个碰撞物体在接触时，其中一个物体在另一个物体内部的距离。
        ///
        /// 当两个物体发生碰撞时，它们的表面通常会部分重叠，导致其中一个物体在另一个物体内部。
        /// 侵入深度就是这种重叠的距离，表示了碰撞表面之间的相互穿透程度。
        ///
        /// 侵入深度是物理引擎用于计算碰撞响应的重要参数之一。
        /// 通过侵入深度，可以确定需要对碰撞进行多少程度的修正，以消除物体之间的相互穿透。修正侵入深度可以防止物体发生穿透现象，使碰撞模拟更加准确。
        ///
        /// mPenetrationDepth 可能是一个标量值，表示接触点的侵入深度。
        /// 通常情况下，侵入深度是一个负值，表示物体之间的相互穿透。物理引擎通常会根据侵入深度来计算碰撞修正力，使物体分离并消除穿透现象。
        /// </summary>
        private float mPenetrationDepth;

        /// <summary>
        /// 在 ContactPoint 中，mLocalPointOnShape1 通常表示接触点在形状 1 的局部坐标系中的位置。
        /// 这个属性描述了碰撞发生时接触点相对于第一个参与碰撞的物体的局部位置。
        ///
        /// 具体来说，mLocalPointOnShape1 是一个三维向量，表示接触点相对于形状 1（例如碰撞体或碰撞形状）局部坐标系的位置。
        /// 在物理引擎中，接触点的局部位置信息对于计算碰撞响应至关重要。根据碰撞发生的具体情况，该向量可以用于确定碰撞点在每个物体上的位置关系，以便进行相应的碰撞解析和碰撞响应计算。
        ///
        /// 总的来说，mLocalPointOnShape1 提供了有关接触点相对于形状 1 的局部坐标系的位置信息，以帮助物理引擎进行准确的碰撞模拟。
        /// </summary>
        private PVector3 mLocalPointOnShape1;

        /// <summary>
        /// 在 ContactPoint 中，mLocalPointOnShape2 通常表示接触点在形状 2 的局部坐标系中的位置。
        /// 这个属性描述了碰撞发生时接触点相对于第一个参与碰撞的物体的局部位置。
        ///
        /// 具体来说，mLocalPointOnShape2 是一个三维向量，表示接触点相对于形状 2（例如碰撞体或碰撞形状）局部坐标系的位置。
        /// 在物理引擎中，接触点的局部位置信息对于计算碰撞响应至关重要。根据碰撞发生的具体情况，该向量可以用于确定碰撞点在每个物体上的位置关系，以便进行相应的碰撞解析和碰撞响应计算。
        ///
        /// 总的来说，mLocalPointOnShape2 提供了有关接触点相对于形状 2 的局部坐标系的位置信息，以帮助物理引擎进行准确的碰撞模拟。
        /// </summary>
        private PVector3 mLocalPointOnShape2;

        /// <summary>
        /// 在物理引擎中，mIsRestingContact 通常表示接触点是否是一个静止接触点（resting contact）。
        /// 静止接触点指的是两个物体之间的接触点，这两个物体在接触点处相对静止，并且没有相对运动。
        ///
        /// 在物理引擎中，区分静止接触点和动态接触点（dynamic contact）对于正确模拟物体之间的碰撞非常重要。
        /// 静止接触点通常不会引起额外的碰撞响应计算，因为两个物体在接触点处已经相对静止。这可以帮助优化物理引擎的性能，减少不必要的计算开销。
        ///
        /// mIsRestingContact 可能是一个布尔值，用于表示接触点是否是一个静止接触点。
        /// 这个属性通常会在物理引擎的碰撞检测和碰撞解析阶段被计算得到，并且用于决定是否需要对接触点进行额外的碰撞响应计算。
        /// </summary>
        private bool mIsRestingContact;

        /// <summary>
        /// 在物理引擎中，mPenetrationImpulse 通常表示解决碰撞时施加于物体的侵入冲量（penetration impulse）。
        /// 侵入冲量是用于解决碰撞中的物体之间的相互穿透问题的一个重要参数。
        ///
        /// 当两个物体发生碰撞并且发生相互穿透时，物理引擎需要施加一个侵入冲量来分离这两个物体，以消除它们之间的相互穿透。
        /// mPenetrationImpulse 表示这个侵入冲量的大小，它通常是一个标量值。
        ///
        /// 侵入冲量的大小通常根据碰撞发生的情况和物体的性质进行计算，以确保物体能够分离并消除相互穿透。
        /// 物理引擎会在碰撞解析阶段计算并施加这个侵入冲量，从而实现物体之间的分离并消除穿透的目的。
        ///
        /// mPenetrationImpulse 是解决碰撞中侵入问题的一个关键参数，它直接影响到碰撞解析的准确性和稳定性。
        /// 因此，在物理引擎中对其进行适当的计算和应用非常重要。
        /// </summary>
        private float mPenetrationImpulse;

        /// <summary>
        /// 在物理引擎中，mIsObsolete 通常表示接触点是否已经过时或无效。
        /// 当物体间的碰撞发生变化，比如一个刚体被移除或者碰撞被解决时，相关的接触点可能会变得无效。
        ///
        /// 在这种情况下，物理引擎通常会将 mIsObsolete 属性设置为 true，表示该接触点已经不再有效。
        /// 这可以帮助物理引擎在后续的更新中识别和处理无效的接触点，从而确保物理模拟的准确性和稳定性。
        ///
        /// 通过检查 mIsObsolete 属性，物理引擎可以避免在无效的接触点上执行不必要的碰撞检测和碰撞解析，从而提高性能并减少计算开销。
        /// 这在具有大量物体和接触点的复杂场景中尤其重要。
        /// </summary>
        private bool mIsObsolete;

        /// <summary>
        /// 如果 mNext 出现在 ContactPoint 中，那么它可能表示一个指向下一个接触点的指针或引用。
        /// 这种设计允许在接触点之间建立链表，以便在物理引擎中对接触点进行遍历或处理。
        /// 
        /// 通过遍历链表，物理引擎可以逐个访问接触点并执行相应的操作，例如碰撞解析或碰撞响应。
        /// 链表的设计使得这种操作变得高效，并且可以方便地处理大量的接触点数据。
        /// </summary>
        private ContactPoint mNext;

        /// <summary>
        /// mPrevious 通常表示当前节点的前一个节点。在上下文中，如果 mPrevious 出现在 ContactPoint 中，那么它可能表示一个指向前一个接触点的指针或引用。
        /// </summary>
        private ContactPoint mPrevious;

        /// <summary>
        /// 在物理引擎中，mPersistentContactDistanceThreshold 通常表示接触点持续存在的距离阈值。
        /// 这个阈值用于确定当两个物体之间的接触点距离超过一定阈值时，是否继续保持它们之间的接触。
        ///
        /// 当两个物体之间发生碰撞时，接触点被创建。然而，由于物体的运动和形变，接触点的位置可能会变化。
        /// 为了避免频繁创建和销毁接触点，物理引擎会根据 mPersistentContactDistanceThreshold 来判断接触点是否应该持续存在。
        ///
        /// 如果接触点的距离超过了 mPersistentContactDistanceThreshold，那么物理引擎可能会认为这个接触点不再有效，并将其销毁。
        /// 反之，如果接触点的距离在阈值之内，那么它可能会被认为是持续存在的，并继续参与后续的碰撞检测和碰撞响应。
        ///
        /// 这个阈值的选择取决于物理引擎的实现和场景的特性，通常会根据物体的运动速度、形变程度以及接触点的稳定性来确定。
        /// 一个合适的阈值可以帮助减少不必要的接触点的创建和销毁，提高物理引擎的性能和稳定性。
        /// </summary>
        private float mPersistentContactDistanceThreshold;
        
        
        public ContactPoint( ContactPointInfo contactInfo, float persistentContactDistanceThreshold)
        {
            mNormal = (contactInfo.normal);
            mPenetrationDepth = (contactInfo.penetrationDepth);
            mLocalPointOnShape1 = (contactInfo.localPoint1);
            mLocalPointOnShape2 = (contactInfo.localPoint2);
            mIsRestingContact = (false); mIsObsolete = (false);
            mPersistentContactDistanceThreshold = (persistentContactDistanceThreshold) ;
            mIsObsolete = false;
        }


        /// <summary>
        /// 这里是在更新接触点，并使用一个新的接触点来替换当前接触点。
        /// 新接触点与当前接触点非常相似（非常接近），目的是保留缓存的冲量（用于热启动接触求解器）。
        ///
        /// 这种做法常见于物理引擎中的接触点管理。在进行迭代求解碰撞时，通常会使用上一帧保存的冲量信息来帮助收敛。
        /// 因此，在更新接触点时，需要确保新接触点的性质与旧接触点相似，以便能够继续使用之前的冲量信息。
        ///
        /// 实际实现中，需要根据当前接触点的属性，如位置、法线方向、侵入深度等，生成一个与之非常相似的新接触点。
        /// 然后将新接触点用于更新当前接触点，并保留之前的缓存冲量信息。
        ///
        /// 这样做有助于保持接触点的稳定性和连续性，并在迭代求解器中提供良好的起始条件，从而加速碰撞解决过程。
        /// </summary>
        private void Update(ContactPointInfo contactInfo)
        {
            mNormal = contactInfo.normal;
            mPenetrationDepth = contactInfo.penetrationDepth;
            mLocalPointOnShape1 = contactInfo.localPoint1;
            mLocalPointOnShape2 = contactInfo.localPoint2;

            mIsObsolete = false;
        }

        /// <summary>
        /// 返回 true，如果接触点与另一个给定的接触点相似（足够接近）。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsSimilarWithContactPoint(ContactPointInfo localContactPointBody1)
        {
            return (localContactPointBody1.localPoint1 - mLocalPointOnShape1).LengthSquare() <= (mPersistentContactDistanceThreshold *
                mPersistentContactDistanceThreshold);
        }

        /// <summary>
        /// 设置缓存的穿透冲量。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetPenetrationImpulse(float impulse)
        {
            mPenetrationImpulse = impulse;
        }

        /// <summary>
        /// 设置 mIsRestingContact 变量。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetIsRestingContact(bool isRestingContact) { mIsRestingContact = isRestingContact; }

        
        /// <summary>
        /// 返回接触的法线向量。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 GetNormal()
        {
            return mNormal;
        }

        /// <summary>
        /// 返回在碰撞体1的局部空间中的第一个碰撞体的接触点。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 GetLocalPointOnShape1() {return mLocalPointOnShape1; }

        /// <summary>
        /// 返回在碰撞体2的局部空间中的第二个碰撞体的接触点。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 GetLocalPointOnShape2() { return mLocalPointOnShape2;}

        /// <summary>
        /// 返回缓存的穿透冲量。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetPenetrationImpulse() {  return mPenetrationImpulse;}

        /// <summary>
        /// 返回 true，如果接触是休息接触。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetIsRestingContact() { return mIsRestingContact;}

        /// <summary>
        /// 返回穿透深度。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetPenetrationDepth() { return mPenetrationDepth;}

        // /// <summary>
        // /// 返回接触点使用的字节数。
        // /// </summary>
        // public int GetSizeInBytes() { return sizeof(ContactPoint);}
    }
}
