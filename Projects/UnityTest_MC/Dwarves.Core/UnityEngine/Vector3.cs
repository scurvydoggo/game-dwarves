namespace UnityEngine
{
    /// <summary>
    /// A dummy class to use when the UnityEngine assembly is missing.
    /// </summary>
    public struct Vector3
    {
        public static readonly Vector3 Zero = new Vector3(0, 0, 0);

        public Vector3(float x, float y, float z)
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }
    }
}