namespace Game.Shared.Constants.Layer {
    public static class LayerConstants {
        // Define the layers as readonly so they can be reused
        public static readonly int OBSTACLE = 1 << 6;           // OBSTACLE is layer 6
        public static readonly int INTERACT = 1 << 7;           // INTERACT is layer 7
        public static readonly int MOVEMENT = 1 << 8;           // MOVEMENT is layer 8
        public static readonly int GROUND = 1 << 9;             // GROUND   is layer 9

        public static readonly int UI_3D_Level_1 = 1 << 29;     // GROUND   is layer 29
        public static readonly int UI_3D_Level_2 = 1 << 30;     // GROUND   is layer 30
        public static readonly int UI_3D_Level_2_INDEX = 30;
        public static readonly int UI_3D_Text = 1 << 31;        // GROUND   is layer 31

        //
        //public static readonly int CombinedLayerMask = GROUND | INTERACT;
        public static readonly int OBSTACLE_AND_INTERACT = OBSTACLE | INTERACT;
        public static readonly int OBSTACLE_INTERACT_GROUND = OBSTACLE | INTERACT | GROUND;

        public static bool IsOnInteractLayer(int layer)
        {
            return (INTERACT & (1 << layer)) != 0;
        }
    }
}
