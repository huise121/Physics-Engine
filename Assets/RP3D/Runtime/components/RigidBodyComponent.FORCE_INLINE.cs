using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RP3D
{
    public partial class RigidBodyComponents
    {
        #region public:

        /// <summary>
        ///     每个组件的实体数组
        /// </summary>
        public List<Entity> mBodiesEntities;

        /// <summary>
        ///     指向相应刚体的指针数组
        /// </summary>
        public List<RigidBody> mRigidBodies;

        /// <summary>
        ///     用于判断物体是否允许进入睡眠状态
        /// </summary>
        public List<bool> mIsAllowedToSleep;

        /// <summary>
        ///     用于判断物体是否处于睡眠状态
        /// </summary>
        public List<bool> mIsSleeping;

        /// <summary>
        ///     在物理引擎中，mSleepTimes（休眠时间）通常指的是刚体进入睡眠状态前需要保持静止的时间阈值。
        ///     当刚体处于静止状态并且在一段时间内没有发生任何运动时，物理引擎会将其置于睡眠状态以节省计算资源。
        ///     mSleepTimes 定义了刚体需要保持静止多长时间后才能进入睡眠状态。
        ///     进入睡眠状态的刚体不再参与碰撞检测和碰撞响应，直到它们被外部力或碰撞事件唤醒。
        ///     这有助于优化物理引擎的性能，尤其是在具有大量刚体的情况下，避免不必要的计算开销。
        ///     mSleepTimes 可能是一个时间阈值，表示刚体需要保持静止的时间量。
        ///     这个时间阈值可以根据物理引擎的设置或者根据刚体的特性进行调整，以平衡模拟的准确性和性能的需求。
        /// </summary>
        public List<float> mSleepTimes;

        /// <summary>
        ///     mBodyTypes 通常表示刚体的类型（body types）。
        ///     在物理引擎中，刚体的类型通常用来描述刚体的特性和行为。
        ///     常见的刚体类型包括：
        ///     静态刚体（Static Body）：静态刚体通常不会移动，其主要作用是提供碰撞体积以及碰撞检测。例如，地面、墙壁等静止不动的物体。
        ///     动态刚体（Dynamic Body）：动态刚体会受到外力的作用而移动，并且可以与其他物体发生碰撞。例如，球、方块等可以被推动、拉动的物体。
        ///     运动学刚体（Kinematic Body）：运动学刚体可以被手动控制移动，并且可以与其他物体发生碰撞。
        ///     与动态刚体不同的是，运动学刚体的位置和速度通常是由外部控制而非物理模拟所确定的。例如，玩家角色、移动平台等。
        ///     mBodyTypes 可能是一个枚举类型或者整数值，用来表示刚体的具体类型。根据不同的物理引擎和实现，可能会有更多特定的刚体类型。
        ///     选择合适的刚体类型对于模拟真实的物理效果和优化物理计算都是非常重要的。
        /// </summary>
        public List<BodyType> mBodyTypes;

        /// <summary>
        ///     如果需要将重力应用于此组件，则为 true
        /// </summary>
        public List<bool> mIsGravityEnabled;

        /// <summary>
        ///     mIsAlreadyInIsland 通常表示刚体是否已经被添加到物理岛（island）中。
        ///     在物理引擎中，物理岛是一组相互连接的刚体，它们之间通过关节、碰撞等相互作用而形成的一个集合。
        ///     物理引擎通常会将刚体按照它们的相互作用关系划分到不同的物理岛中，以便进行高效的物理模拟。
        ///     当刚体已经被添加到物理岛中时，mIsAlreadyInIsland 会被设置为true；否则，它会被设置为false。
        ///     这个属性通常用于物理引擎内部的算法中，以确保刚体被正确地管理和模拟。
        ///     例如，在执行物理模拟时，物理引擎可能会首先检查刚体是否已经被添加到物理岛中，以避免重复添加或处理。
        /// </summary>
        public List<bool> mIsAlreadyInIsland;

        /// <summary>
        ///     mJoints 通常表示关节（joints）。
        ///     在物理引擎中，关节是用于连接两个或多个刚体以模拟它们之间的约束或连接的一种机制。
        ///     关节可以模拟多种约束，如固定关节（固定两个刚体的相对位置和方向）、球关节（允许两个刚体在球形范围内自由旋转）、
        ///     铰链关节（只允许两个刚体在一个轴上自由旋转）、滑轮关节（允许两个刚体沿一个轴相对滑动）等。
        ///     mJoints 可能是一个列表或数组，其中每个元素代表一个关节。
        ///     每个关节通常包含连接的刚体、约束类型、约束参数等信息。
        ///     关节在物理引擎中起着至关重要的作用，它们可以用来模拟各种物理现象，如机械装置、关节连接的生物体等。
        ///     物理引擎会根据关节的约束条件和参数来计算刚体的运动，并模拟出真实的物理效果。
        /// </summary>
        public List<List<Entity>> mJoints;

        /// <summary>
        ///     mContactPairs 通常表示接触对（contact pairs）。
        ///     在物理引擎中，接触对是指两个物体之间存在接触的情况。
        ///     当两个物体之间发生碰撞或接触时，物理引擎会检测到这种接触，并将它们称为接触对。
        ///     mContactPairs 可能是一个列表或数组，其中每个元素都表示一个接触对。
        ///     每个接触对通常包含两个物体以及它们之间的接触信息，如接触点、法线、深度等。
        ///     接触对在物理引擎中非常重要，因为它们决定了物体之间的碰撞响应。
        ///     物理引擎会根据接触对的信息计算碰撞力，并将其应用于相应的物体上，从而模拟出真实的碰撞效果。
        /// </summary>
        public List<List<int>> mContactPairs;


        /// <summary>
        ///     指的是物体的线性速度。
        ///     在物理引擎中，线性速度是描述物体在三维空间中移动的速度。
        ///     这个速度通常包括了物体在三个坐标轴上的运动速度分量：沿着 x、y 和 z 轴的速度。
        ///     这些速度分量可以表示为一个三维向量，即物体的线性速度向量。
        ///     在 mLinearVelocities 中，可能包含了系统中所有物体的线性速度信息，每个物体的线性速度可能存储为一个向量。
        ///     这些线性速度可能在物理仿真的不同阶段被更新和修改，以反映物体在模拟中的运动和受力情况。
        /// </summary>
        public List<PVector3> mLinearVelocities;

        /// <summary>
        ///     在物理引擎中，mAngularVelocities通常表示刚体的角速度（angular velocities）。
        ///     角速度是描述刚体绕其质心旋转的速度。对于三维空间中的刚体，角速度通常是一个三维向量，每个分量表示绕对应坐标轴旋转的速度。
        ///     例如，对于一个刚体在三维空间中的运动，其角速度向量可以表示为 (wx, wy, wz)，
        ///     其中 wx 表示绕 X 轴的旋转速度，wy 表示绕 Y 轴的旋转速度，wz 表示绕 Z 轴的旋转速度。
        ///     在物理引擎中，角速度向量通常用于模拟刚体的自由旋转运动，参与到碰撞检测、约束解决等物理计算中。
        /// </summary>
        public List<PVector3> mAngularVelocities;

        /// <summary>
        ///     mExternalForces 通常表示刚体受到的外部力（external forces）。
        ///     在物理引擎中，力是导致物体加速度改变的原因，它可以是来自外部环境的作用力，比如重力、风力、施加的推力等。
        ///     mExternalForces 可能是一个三维向量，表示物体受到的总外部力在各个坐标轴上的分量，即 x、y 和 z 方向上的力。
        ///     这些外部力会影响物体的运动状态，使其产生加速度，从而改变其速度和位置。
        ///     在物理引擎中，外部力是模拟物体在三维空间中运动和相互作用的重要因素之一。
        ///     物理引擎会根据外部力的作用计算物体的加速度，并据此更新物体的位置和速度。
        /// </summary>
        public List<PVector3> mExternalForces;

        /// <summary>
        ///     mExternalTorques 通常表示刚体受到的外部扭矩（external torques）。
        ///     在物理引擎中，扭矩是一个力矩，它产生了绕某一轴旋转的趋势。
        ///     在刚体的运动中，除了受到外部的力以外，还可能受到外部扭矩的作用，例如风力、摩擦力等。
        ///     mExternalTorques 可以是一个三维向量，表示绕各个坐标轴的扭矩分量，也就是绕 x、y 和 z 轴的旋转趋势。
        ///     在物理引擎中，外部扭矩会影响刚体的角加速度，从而改变其角速度和旋转状态。
        ///     这些扭矩在模拟刚体的旋转运动、角动量的变化等方面发挥着重要作用。
        /// </summary>
        public List<PVector3> mExternalTorques;

        /// <summary>
        ///     mLinearDampings 通常表示刚体的线性阻尼（linear damping）。
        ///     在物理引擎中，阻尼是一种用于模拟物体受到的空气阻力或其他介质阻力的力的效果。
        ///     线性阻尼作用于刚体的线性速度，减慢其运动速度并最终使其停止。
        ///     mLinearDampings 可能是一个三维向量，表示在各个坐标轴上的线性阻尼系数，即 x、y 和 z 方向上的阻尼系数。
        ///     线性阻尼通常用于模拟现实世界中的物体运动，使物体在没有外部力作用时逐渐减速并停止运动。
        ///     在物理引擎中，线性阻尼的计算通常与物体的质量、速度和其他因素相结合，以模拟真实世界中的运动行为。
        /// </summary>
        public List<float> mLinearDampings;

        /// <summary>
        ///     mAngularDampings 通常表示刚体的角阻尼（angular damping）。
        ///     在物理引擎中，角阻尼是一种模拟物体围绕其质心旋转时受到的阻力效果。
        ///     与线性阻尼类似，角阻尼作用于刚体的角速度，减慢其旋转速度并最终使其停止旋转。
        ///     mAngularDampings 可能是一个三维向量，表示绕各个坐标轴的角阻尼系数，即绕 x、y 和 z 轴的角阻尼系数。
        ///     角阻尼通常用于模拟真实世界中的旋转运动，例如飞行器在空气中受到的阻力效果，或者滚动物体在地面上滚动时的摩擦力。
        ///     在物理引擎中，角阻尼的计算通常与物体的惯性、角速度和其他因素相结合，以模拟真实世界中的旋转行为。
        /// </summary>
        public List<float> mAngularDampings;

        /// <summary>
        ///     mMasses 通常表示刚体的质量属性（mass）。
        ///     在物理引擎中，质量是一个物体所具有的惯性和受力响应的属性。
        ///     mMasses 可能是一个标量值，表示刚体的总质量，也可能是一个向量，用于表示物体在不同方向上的分布质量。
        ///     质量在物理引擎中是一个非常重要的参数，它影响了物体对外部力的响应以及与其他物体的碰撞行为。
        ///     通常来说，质量越大的物体，对外部力的响应越小，惯性越大，因此需要更多的力才能改变其运动状态。
        ///     在物理引擎中，质量还会影响到刚体的加速度和动量变化。
        ///     通过质量属性，物理引擎可以模拟物体在碰撞、受力等情况下的运动行为，使得仿真结果更加真实可信。
        /// </summary>
        public List<float> mMasses;

        /// <summary>
        ///     mInverseMasses 通常表示刚体的质量的倒数（inverse mass）。
        ///     在物理引擎中，有时候使用质量的倒数来代替质量本身进行计算，这是因为在物理计算中，需要频繁地进行除法操作，而除法的计算成本比乘法高得多。
        ///     因此，将质量的倒数存储下来可以减少一些重复的计算，并且在物理引擎的某些算法中，使用质量的倒数更为方便。
        ///     质量的倒数通常用来表示物体的反应能力。如果一个物体的质量很大，其质量的倒数就会很小，意味着它对外部力的响应很小；
        ///     相反，如果一个物体的质量很小，其质量的倒数就会很大，意味着它对外部力的响应很大。
        ///     在物理引擎中，质量的倒数与质量一样，是物体的一个重要属性，用于模拟物体的运动行为和碰撞响应。
        /// </summary>
        public List<float> mInverseMasses;

        /// <summary>
        ///     mLocalInertiaTensors 通常表示刚体的局部惯性张量（local inertia tensors）。
        ///     在物理引擎中，惯性张量是描述物体在旋转运动中的惯性特性的数学概念。
        ///     刚体的局部惯性张量是一个 3x3 的矩阵，它描述了物体围绕其质心旋转时的惯性分布情况。
        ///     这个矩阵的每个元素表示了物体在不同方向上的惯性。
        ///     惯性张量对于模拟刚体的旋转运动非常重要。
        ///     在物理引擎中，它们通常被用于计算刚体在受到外部扭矩时的角加速度，以及在碰撞中计算刚体的旋转响应。
        ///     通常情况下，刚体的惯性张量是相对于其本地坐标系来计算的，因此称为局部惯性张量。
        ///     在物理引擎中，这些惯性张量通常是在刚体建立时计算得到，并存储在 mLocalInertiaTensors 中，以便在模拟过程中使用。
        /// </summary>
        public List<PVector3> mLocalInertiaTensors;

        /// <summary>
        ///     mInverseInertiaTensorsLocal 通常表示刚体的局部惯性张量的逆矩阵（inverse inertia tensors）。
        ///     在物理引擎中，刚体的惯性张量用于描述物体围绕其质心旋转时的惯性特性，而其逆矩阵用于描述物体对外部扭矩的响应。
        ///     逆惯性张量矩阵是惯性张量矩阵的逆。
        ///     当物体受到扭矩时，逆惯性张量矩阵被用于计算刚体的角加速度。
        ///     通过将扭矩乘以逆惯性张量矩阵，可以得到刚体的角加速度。
        ///     在物理引擎中，逆惯性张量矩阵通常与惯性张量一起被计算，并存储在 mInverseInertiaTensorsLocal 中，以便在模拟过程中使用。
        ///     这个矩阵对于模拟刚体的旋转运动非常重要，因为它决定了刚体在受到外部扭矩时的旋转响应。
        /// </summary>
        public List<PVector3> mInverseInertiaTensorsLocal;

        /// <summary>
        ///     mInverseInertiaTensorsWorld 通常表示刚体的全局惯性张量的逆矩阵（inverse inertia tensors in world space）。
        ///     在物理引擎中，全局惯性张量是描述物体围绕其质心旋转时的惯性特性的数学概念。
        ///     与局部惯性张量不同，全局惯性张量考虑了刚体在世界坐标系下的旋转情况。
        ///     刚体在旋转时，其局部坐标系可能会随之变化，而全局惯性张量则是相对于世界坐标系来定义的。
        ///     mInverseInertiaTensorsWorld 是全局惯性张量的逆矩阵，它描述了物体对于外部扭矩的响应。
        ///     当刚体受到扭矩时，逆惯性张量矩阵被用于计算刚体的角加速度。通过将扭矩乘以逆惯性张量矩阵，可以得到刚体的角加速度。
        ///     在物理引擎中，全局惯性张量和其逆矩阵通常在刚体建立时计算得到，并随着刚体的旋转而更新。
        ///     这些矩阵对于模拟刚体的旋转运动非常重要，因为它们决定了刚体在受到外部扭矩时的旋转响应。
        /// </summary>
        public List<Matrix3x3> mInverseInertiaTensorsWorld;

        /// <summary>
        ///     指的是受到约束后的线性速度。
        ///     在物理引擎中，约束通常用于限制物体在某些方面的运动，例如保持关节的角度或限制物体的运动范围。
        ///     mConstrainedLinearVelocities 可能是一个数组或列表，存储了受约束的物体的线性速度。
        ///     这些速度可能已经经过某种方式的限制或调整，以确保物体在约束下的运动符合物理规则和约束条件。
        /// </summary>
        public List<PVector3> mConstrainedLinearVelocities;

        /// <summary>
        ///     指的是受到约束后的角速度。
        ///     在物理引擎中，当物体受到约束时，其旋转运动可能会受到限制或调整，以满足约束条件。
        ///     mConstrainedAngularVelocities 可能是一个数组或列表，用于存储受约束的物体的角速度。
        ///     这些角速度可能已经经过某种方式的限制或调整，以确保物体在约束下的旋转运动符合物理规则和约束条件。
        /// </summary>
        public List<PVector3> mConstrainedAngularVelocities;

        /// <summary>
        ///     mSplitLinearVelocities 是指刚体线性速度的分离部分。
        ///     在物理引擎中，刚体的线性速度通常可以分为两个部分：一部分是由于刚体的整体运动而产生的线性速度，另一部分是由于刚体在旋转运动时所产生的线性速度。
        ///     因此，mSplitLinearVelocities 可能用来表示这两部分线性速度的分离。
        ///     这在一些物理引擎中可能用于优化计算，或者用于特定的物理模拟场景中。
        ///     在物理引擎中，对刚体的运动进行建模时，通常需要考虑其平移运动和旋转运动对线性速度的影响，mSplitLinearVelocities 可能用于描述这种分离情况。
        /// </summary>
        public List<PVector3> mSplitLinearVelocities;

        /// <summary>
        ///     mSplitAngularVelocities 通常表示刚体的角速度的分离部分。
        ///     在物理引擎中，刚体的角速度通常可以分为两个部分：一部分是由于刚体的整体旋转而产生的角速度，另一部分是由于刚体在平移运动时所产生的角速度。
        ///     因此，mSplitAngularVelocities 可能用来表示这两部分角速度的分离。
        ///     这在一些物理引擎中可能用于优化计算，或者用于特定的物理模拟场景中。
        ///     在物理引擎中，对刚体的运动进行建模时，通常需要考虑其平移运动和旋转运动对角速度的影响，mSplitAngularVelocities 可能用于描述这种分离情况。
        /// </summary>
        public List<PVector3> mSplitAngularVelocities;

        /// <summary>
        ///     mConstrainedPositions 通常表示受约束的位置（constrained positions）。
        ///     在物理引擎中，刚体之间的约束（例如关节、距离约束等）会影响它们的运动，限制它们的位置或者相对位置。
        ///     这些受约束的位置是经过约束求解器计算得到的，在满足约束条件的情况下，确定刚体的位置。
        ///     mConstrainedPositions 通常是一个向量或者矩阵，用来存储受到约束影响后的刚体位置。
        ///     这些位置通常被用于后续的物理模拟计算，以确保刚体在满足约束的情况下进行运动。
        ///     受约束的位置在物理引擎中是非常重要的，因为它们决定了刚体在考虑约束的情况下如何移动。
        ///     物理引擎会根据约束求解器得到的受约束的位置来计算刚体的受力和加速度，从而模拟出刚体的运动行为。
        /// </summary>
        public List<PVector3> mConstrainedPositions;

        /// <summary>
        ///     mConstrainedOrientations 通常表示受约束的方向或者姿态（constrained orientations）。
        ///     在物理引擎中，刚体之间的约束（例如关节、旋转约束等）会影响它们的旋转，限制它们的方向或者相对方向。
        ///     这些受约束的方向或姿态是经过约束求解器计算得到的，在满足约束条件的情况下，确定刚体的旋转。
        ///     mConstrainedOrientations 通常是一个向量、矩阵或四元数，用来存储受到约束影响后的刚体方向或者姿态。
        ///     这些方向或姿态通常被用于后续的物理模拟计算，以确保刚体在满足约束的情况下进行旋转。
        ///     受约束的方向或姿态在物理引擎中同样是非常重要的，因为它们决定了刚体在考虑约束的情况下如何旋转。
        ///     物理引擎会根据约束求解器得到的受约束的方向或姿态来计算刚体的受力和角加速度，从而模拟出刚体的旋转行为。
        /// </summary>
        public List<PQuaternion> mConstrainedOrientations;

        /// <summary>
        ///     mCentersOfMassLocal 通常表示刚体的局部质心位置（centers of mass in local space）。
        ///     在物理引擎中，质心是物体的几何中心，即物体的质量均匀分布时的位置。而局部质心位置表示的是相对于物体自身局部坐标系的质心位置。
        ///     mCentersOfMassLocal 可能是一个向量，用来描述刚体的局部质心相对于刚体自身的位置。
        ///     这个位置通常用来计算刚体的运动学和动力学属性，比如惯性张量、旋转惯量等。
        ///     刚体的质心位置对于模拟物体的运动和碰撞行为非常重要。
        ///     在物理引擎中，质心位置通常被用来计算刚体的惯性属性，并参与到碰撞检测、碰撞响应等物理计算中。
        /// </summary>
        public List<PVector3> mCentersOfMassLocal;

        /// <summary>
        ///     mCentersOfMassWorld 通常表示刚体的全局质心位置（centers of mass in world space）。
        ///     在物理引擎中，质心是物体的几何中心，即物体的质量均匀分布时的位置。而全局质心位置表示的是相对于世界坐标系的质心位置。
        ///     mCentersOfMassWorld 可能是一个向量，用来描述刚体的全局质心位置。
        ///     这个位置通常用来计算刚体的运动学和动力学属性，比如惯性张量、旋转惯量等。
        ///     刚体的质心位置对于模拟物体的运动和碰撞行为非常重要。在物理引擎中，质心位置通常被用来计算刚体的惯性属性，并参与到碰撞检测、碰撞响应等物理计算中。
        ///     因为在世界坐标系中描述的质心位置能够让我们更容易地与其他物体进行碰撞检测和碰撞响应。
        /// </summary>
        public List<PVector3> mCentersOfMassWorld;

        /// <summary>
        ///     mLinearLockAxisFactors 通常表示刚体的线性锁定轴因子（linear lock axis factors）。
        ///     在物理引擎中，有时需要限制刚体在某些方向上的运动，以模拟约束或限制条件。
        ///     这些因子通常是一组值，用来表示在每个坐标轴上应用的线性锁定约束的程度。
        ///     例如，如果某个坐标轴的线性锁定轴因子为1，意味着该轴上的线性运动被完全锁定，而如果为0，意味着该轴上的线性运动不受任何限制。
        ///     线性锁定轴因子通常用于约束刚体在特定方向上的运动，例如限制物体在某些方向上的位移。
        ///     这些因子可以用于实现一些物理效果，例如模拟关节的运动范围或者限制刚体的移动自由度。
        /// </summary>
        public List<PVector3> mLinearLockAxisFactors;

        /// <summary>
        ///     mAngularLockAxisFactors 通常表示刚体的角度锁定轴因子（angular lock axis factors）。
        ///     在物理引擎中，有时需要限制刚体在某些方向上的旋转，以模拟约束或限制条件。
        ///     这些因子通常是一组值，用来表示在每个轴上应用的角度锁定约束的程度。
        ///     例如，如果某个轴的角度锁定轴因子为1，意味着该轴上的旋转被完全锁定，而如果为0，意味着该轴上的旋转不受任何限制。
        ///     角度锁定轴因子通常用于约束刚体在特定方向上的旋转，例如限制物体绕某些轴的旋转。
        ///     这些因子可以用于实现一些物理效果，例如模拟关节的运动范围或者限制刚体的旋转自由度。
        /// </summary>
        public List<PVector3> mAngularLockAxisFactors;

        #endregion

        
        #region inline

        // 返回指向刚体的指针
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RigidBody GetRigidBody(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mRigidBodies[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 返回是否允许刚体进入睡眠状态
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetIsAllowedToSleep(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mIsAllowedToSleep[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 设置刚体是否允许进入睡眠状态的值
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetIsAllowedToSleep(Entity bodyEntity, bool isAllowedToSleep)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mIsAllowedToSleep[mMapEntityToComponentIndex[bodyEntity]] = isAllowedToSleep;
        }

        // 返回刚体是否处于睡眠状态
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetIsSleeping(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mIsSleeping[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 设置刚体是否处于睡眠状态的值
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetIsSleeping(Entity bodyEntity, bool isSleeping)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mIsSleeping[mMapEntityToComponentIndex[bodyEntity]] = isSleeping;
        }

        // 返回睡眠时间
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetSleepTime(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mSleepTimes[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 设置睡眠时间
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSleepTime(Entity bodyEntity, float sleepTime)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mSleepTimes[mMapEntityToComponentIndex[bodyEntity]] = sleepTime;
        }

        // 返回刚体的类型
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BodyType GetBodyType(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mBodyTypes[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 设置刚体的类型
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetBodyType(Entity bodyEntity, BodyType bodyType)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mBodyTypes[mMapEntityToComponentIndex[bodyEntity]] = bodyType;
        }

        // 返回实体的线性速度
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 GetLinearVelocity(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mLinearVelocities[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 返回实体的角速度
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 GetAngularVelocity(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mAngularVelocities[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 设置实体的线性速度
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetLinearVelocity(Entity bodyEntity, PVector3 linearVelocity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mLinearVelocities[mMapEntityToComponentIndex[bodyEntity]] = linearVelocity;
        }

        // 设置实体的角速度
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetAngularVelocity(Entity bodyEntity, PVector3 angularVelocity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mAngularVelocities[mMapEntityToComponentIndex[bodyEntity]] = angularVelocity;
        }

        // 返回实体的外部力
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 GetExternalForce(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mExternalForces[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 返回实体的外部扭矩
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 GetExternalTorque(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mExternalTorques[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 返回实体的线性阻尼系数
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetLinearDamping(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mLinearDampings[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 返回实体的角阻尼系数
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetAngularDamping(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mAngularDampings[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 返回实体的质量
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetMass(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mMasses[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 返回实体的质量的倒数
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetMassInverse(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mInverseMasses[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 返回实体的局部惯性张量的倒数
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 GetInertiaTensorLocalInverse(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mInverseInertiaTensorsLocal[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 返回实体的世界惯性张量的倒数
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix3x3 GetInertiaTensorWorldInverse(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mInverseInertiaTensorsWorld[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 设置实体的外部力
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetExternalForce(Entity bodyEntity, PVector3 externalForce)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mExternalForces[mMapEntityToComponentIndex[bodyEntity]] = externalForce;
        }

        // 设置实体的外部扭矩
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetExternalTorque(Entity bodyEntity, PVector3 externalTorque)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mExternalTorques[mMapEntityToComponentIndex[bodyEntity]] = externalTorque;
        }

        // 设置实体的线性阻尼系数
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetLinearDamping(Entity bodyEntity, float linearDamping)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mLinearDampings[mMapEntityToComponentIndex[bodyEntity]] = linearDamping;
        }

        // 设置实体的角阻尼系数
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetAngularDamping(Entity bodyEntity, float angularDamping)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mAngularDampings[mMapEntityToComponentIndex[bodyEntity]] = angularDamping;
        }

        // 设置实体的质量
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetMass(Entity bodyEntity, float mass)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mMasses[mMapEntityToComponentIndex[bodyEntity]] = mass;
        }

        // 设置实体的质量的倒数
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetMassInverse(Entity bodyEntity, float inverseMass)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mInverseMasses[mMapEntityToComponentIndex[bodyEntity]] = inverseMass;
        }

        // 返回实体的局部惯性张量
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 GetLocalInertiaTensor(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mLocalInertiaTensors[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 设置实体的局部惯性张量
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetLocalInertiaTensor(Entity bodyEntity, PVector3 inertiaTensorLocal)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mLocalInertiaTensors[mMapEntityToComponentIndex[bodyEntity]] = inertiaTensorLocal;
        }

        // 设置实体的局部惯性张量的倒数
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetInverseInertiaTensorLocal(Entity bodyEntity, PVector3 inertiaTensorLocalInverse)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mInverseInertiaTensorsLocal[mMapEntityToComponentIndex[bodyEntity]] = inertiaTensorLocalInverse;
        }

        // 返回实体的受限线性速度
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 GetConstrainedLinearVelocity(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mConstrainedLinearVelocities[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 返回实体的受限角速度
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 GetConstrainedAngularVelocity(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mConstrainedAngularVelocities[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 返回实体的分离线性速度
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 GetSplitLinearVelocity(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mSplitLinearVelocities[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 返回实体的分离角速度
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 GetSplitAngularVelocity(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mSplitAngularVelocities[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 返回实体的受限位置
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 GetConstrainedPosition(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mConstrainedPositions[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 返回实体的受限方向
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PQuaternion GetConstrainedOrientation(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mConstrainedOrientations[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 返回实体的局部质心
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 GetCenterOfMassLocal(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mCentersOfMassLocal[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 返回实体的世界质心
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 GetCenterOfMassWorld(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mCentersOfMassWorld[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 设置实体的受限线性速度
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetConstrainedLinearVelocity(Entity bodyEntity, PVector3 constrainedLinearVelocity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mConstrainedLinearVelocities[mMapEntityToComponentIndex[bodyEntity]] = constrainedLinearVelocity;
        }

        // 设置实体的受限角速度
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetConstrainedAngularVelocity(Entity bodyEntity, PVector3 constrainedAngularVelocity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mConstrainedAngularVelocities[mMapEntityToComponentIndex[bodyEntity]] = constrainedAngularVelocity;
        }

        // 设置实体的分离线性速度
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSplitLinearVelocity(Entity bodyEntity, PVector3 splitLinearVelocity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mSplitLinearVelocities[mMapEntityToComponentIndex[bodyEntity]] = splitLinearVelocity;
        }

        // 设置实体的分离角速度
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetSplitAngularVelocity(Entity bodyEntity, PVector3 splitAngularVelocity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mSplitAngularVelocities[mMapEntityToComponentIndex[bodyEntity]] = splitAngularVelocity;
        }

        // 设置实体的受限位置
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetConstrainedPosition(Entity bodyEntity, PVector3 constrainedPosition)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mConstrainedPositions[mMapEntityToComponentIndex[bodyEntity]] = constrainedPosition;
        }

        // 设置实体的受限方向
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetConstrainedOrientation(Entity bodyEntity, PQuaternion constrainedOrientation)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mConstrainedOrientations[mMapEntityToComponentIndex[bodyEntity]] = constrainedOrientation;
        }

        // 设置实体的局部质心
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetCenterOfMassLocal(Entity bodyEntity, PVector3 centerOfMassLocal)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mCentersOfMassLocal[mMapEntityToComponentIndex[bodyEntity]] = centerOfMassLocal;
        }

        // 设置实体的世界质心
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetCenterOfMassWorld(Entity bodyEntity, PVector3 centerOfMassWorld)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mCentersOfMassWorld[mMapEntityToComponentIndex[bodyEntity]] = centerOfMassWorld;
        }

        // 返回是否启用重力对此实体
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetIsGravityEnabled(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mIsGravityEnabled[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 返回实体是否已经在一个岛中
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetIsAlreadyInIsland(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mIsAlreadyInIsland[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 返回线性锁定轴因子
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 GetLinearLockAxisFactor(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mLinearLockAxisFactors[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 返回角锁定轴因子
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PVector3 GetAngularLockAxisFactor(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mAngularLockAxisFactors[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 设置是否启用重力对此实体
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetIsGravityEnabled(Entity bodyEntity, bool isGravityEnabled)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mIsGravityEnabled[mMapEntityToComponentIndex[bodyEntity]] = isGravityEnabled;
        }

        // 设置实体是否已经在一个岛中
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetIsAlreadyInIsland(Entity bodyEntity, bool isAlreadyInIsland)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mIsAlreadyInIsland[mMapEntityToComponentIndex[bodyEntity]] = isAlreadyInIsland;
        }

        // 设置线性锁定轴因子
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetLinearLockAxisFactor(Entity bodyEntity, PVector3 linearLockAxisFactor)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mLinearLockAxisFactors[mMapEntityToComponentIndex[bodyEntity]] = linearLockAxisFactor;
        }

        // 设置角锁定轴因子
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetAngularLockAxisFactor(Entity bodyEntity, PVector3 angularLockAxisFactor)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mAngularLockAxisFactors[mMapEntityToComponentIndex[bodyEntity]] = angularLockAxisFactor;
        }

        // 返回身体的关节数组
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<Entity> GetJoints(Entity bodyEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            return mJoints[mMapEntityToComponentIndex[bodyEntity]];
        }

        // 将关节添加到身体组件
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddJointToBody(Entity bodyEntity, Entity jointEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mJoints[mMapEntityToComponentIndex[bodyEntity]].Add(jointEntity);
        }

        // 从身体组件中删除关节
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveJointFromBody(Entity bodyEntity, Entity jointEntity)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mJoints[mMapEntityToComponentIndex[bodyEntity]].Remove(jointEntity);
        }

        // 将关联的接触对添加到身体的接触对数组中
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddContactPair(Entity bodyEntity, int contactPairIndex)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mContactPairs[mMapEntityToComponentIndex[bodyEntity]].Add(contactPairIndex);
        }

        //将一个关联联系人对添加到主体的联系人对数组中
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddContacPair(Entity bodyEntity, int contactPairIndex)
        {
            assert(mMapEntityToComponentIndex.ContainsKey(bodyEntity));
            mContactPairs[mMapEntityToComponentIndex[bodyEntity]].Add(contactPairIndex);
        }

        #endregion
    }
}