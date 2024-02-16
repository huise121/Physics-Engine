namespace RP3D
{
    public static class configuration
    {
        public static uint NB_MAX_CONTACT_MANIFOLDS = 3;

        public static uint NB_MAX_POTENTIAL_CONTACT_MANIFOLDS = 4 * NB_MAX_CONTACT_MANIFOLDS;

        public enum  ContactsPositionCorrectionTechnique {BAUMGARTE_CONTACTS, SPLIT_IMPULSES};

        public static int NB_MAX_CONTACT_POINTS_IN_POTENTIAL_MANIFOLD = 256;

    }
}