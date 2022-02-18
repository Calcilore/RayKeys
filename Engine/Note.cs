namespace RayKeys {
    public class Note {
        public float time;
        public byte lane;
        public bool dead;

        public Note(float t, byte l) {
            time = t; lane = l;
        }
    }
}   