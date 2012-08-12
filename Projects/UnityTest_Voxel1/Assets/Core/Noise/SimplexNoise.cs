// ----------------------------------------------------------------------------
// <copyright file="SimplexNoise.cs" company="Acidwashed Games">
//     Copyright 2012 Acidwashed Games. All right reserved.
// </copyright>
// ----------------------------------------------------------------------------

/// <summary>
/// Generates Simplex Noise.
/// </summary>
public class SimplexNoise
{
    #region Private Variables

    /// <summary>
    /// Permutations array.
    /// </summary>
    private static byte[] perm = new byte[512]
    {
        151,160,137,91,90,15,
        131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
        190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
        88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
        77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
        102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
        135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
        5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
        223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
        129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
        251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
        49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
        138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
        151,160,137,91,90,15,
        131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
        190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
        88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
        77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
        102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
        135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
        5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
        223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
        129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
        251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
        49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
        138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180 
    };

    #endregion

    #region Public Methods

    /// <summary>
    /// Generate 1D simplex noise.
    /// </summary>
    /// <param name="x">The x position.</param>
    /// <returns>The noise value.</returns>
    public static float Generate(float x)
    {
        // Get the points before and after x
        int i0 = FastFloor(x);
        int i1 = i0 + 1;

        // Get the offsets from the points before and after x
        float x0 = x - i0;
        float x1 = x0 - 1;

        // Calculate the noise at the first point
        float t0 = 1 - (x0 * x0);
        t0 *= t0;
        float n0 = t0 * t0 * GetGradient(perm[i0 & 0xFF], x0);

        // Calculate the noise at the second point
        float t1 = 1 - (x1 * x1);
        t1 *= t1;
        float n1 = t1 * t1 * GetGradient(perm[i1 & 0xFF], x1);

        // The maximum value of this noise is 8*(3/4)^4 = 2.53125
        // A factor of 0.395 scales to fit exactly within [-1,1]
        return 0.395f * (n0 + n1);
    }

    /// <summary>
    /// Generate 2D simplex noise.
    /// </summary>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <returns>The noise value.</returns>
    public static float Generate(float x, float y)
    {
        // Skewing factors for the 2D case
        // A step of (1,0) in (i,j) means a step of (1-c,-c) in (x,y), and
        // a step of (0,1) in (i,j) means a step of (-c,1-c) in (x,y), where
        // c = (3-sqrt(3))/6
        const float F2 = 0.366025403f; // F2 = 0.5*(sqrt(3.0)-1.0)
        const float G2 = 0.211324865f; // G2 = (3.0-Math.sqrt(3.0))/6.0

        float n0, n1, n2; // Noise contributions from the three corners

        // Skew the input space to determine which simplex cell we're in
        float s = (x + y) * F2;
        float xs = x + s;
        float ys = y + s;
        int i = FastFloor(xs);
        int j = FastFloor(ys);

        // Unskew the cell origin back to (x,y) space
        float t = (float)(i + j) * G2;
        float originX = i - t;
        float originY = j - t;

        // The x,y distances from the cell origin
        float x0 = x - originX;
        float y0 = y - originY;

        // For the 2D case, the simplex shape is an equilateral triangle. Determine which simplex we are in
        int i1, j1; // Offsets for second (middle) corner of simplex in (i,j) coords
        if (x0 > y0)
        {
            // Lower triangle, XY order: (0,0)->(1,0)->(1,1)
            i1 = 1;
            j1 = 0;
        }
        else
        {
            // Upper triangle, YX order: (0,0)->(0,1)->(1,1)
            i1 = 0;
            j1 = 1;
        }

        // Offsets for middle corner in (x,y) unskewed coords
        float x1 = x0 - i1 + G2;
        float y1 = y0 - j1 + G2;

        // Offsets for last corner in (x,y) unskewed coords
        float x2 = x0 - 1 + (2 * G2);
        float y2 = y0 - 1 + (2 * G2);

        // Wrap the integer indices at 256, to avoid indexing perm[] out of bounds
        int ii = i % 256;
        int jj = j % 256;

        // Calculate the contribution from the three corners
        float t0 = 0.5f - (x0 * x0) - (y0 * y0);
        if (t0 < 0)
        {
            n0 = 0;
        }
        else
        {
            t0 *= t0;
            n0 = t0 * t0 * GetGradient(perm[ii + perm[jj]], x0, y0);
        }

        float t1 = 0.5f - (x1 * x1) - (y1 * y1);
        if (t1 < 0)
        {
            n1 = 0;
        }
        else
        {
            t1 *= t1;
            n1 = t1 * t1 * GetGradient(perm[ii + i1 + perm[jj + j1]], x1, y1);
        }

        float t2 = 0.5f - (x2 * x2) - (y2 * y2);
        if (t2 < 0)
        {
            n2 = 0;
        }
        else
        {
            t2 *= t2;
            n2 = t2 * t2 * GetGradient(perm[ii + 1 + perm[jj + 1]], x2, y2);
        }

        // Add contributions from each corner to get the final noise value.
        // The result is scaled to return values in the interval [-1,1].
        return 40 * (n0 + n1 + n2); // TODO: The scale factor is preliminary!
    }

    /// <summary>
    /// Generate 3D simplex noise.
    /// </summary>
    /// <param name="x">The x position.</param>
    /// <param name="y">The y position.</param>
    /// <param name="z">The z position.</param>
    /// <returns>The noise value.</returns>
    public static float Generate(float x, float y, float z)
    {
        // Skewing factors for the 3D case
        const float F3 = 0.333333333f;
        const float G3 = 0.166666667f;

        float n0, n1, n2, n3; // Noise contributions from the four corners

        // Skew the input space to determine which simplex cell we're in
        float s = (x + y + z) * F3; // Very nice and simple skew factor for 3D
        float xs = x + s;
        float ys = y + s;
        float zs = z + s;
        int i = FastFloor(xs);
        int j = FastFloor(ys);
        int k = FastFloor(zs);

        // Unskew the cell origin back to (x,y,z) space
        float t = (float)(i + j + k) * G3;
        float originX = i - t;
        float originY = j - t;
        float originZ = k - t;

        // The x,y,z distances from the cell origin
        float x0 = x - originX;
        float y0 = y - originY;
        float z0 = z - originZ;

        // For the 3D case, the simplex shape is a slightly irregular tetrahedron.
        // Determine which simplex we are in.
        int i1, j1, k1; // Offsets for second corner of simplex in (i,j,k) coords
        int i2, j2, k2; // Offsets for third corner of simplex in (i,j,k) coords

        /* This code would benefit from a backport from the GLSL version! */
        if (x0 >= y0)
        {
            if (y0 >= z0)
            {
                // X Y Z order
                i1 = 1;
                j1 = 0;
                k1 = 0;
                i2 = 1;
                j2 = 1;
                k2 = 0;
            }
            else if (x0 >= z0)
            {
                // X Z Y order
                i1 = 1;
                j1 = 0;
                k1 = 0;
                i2 = 1;
                j2 = 0;
                k2 = 1;
            }
            else
            {
                // Z X Y order
                i1 = 0;
                j1 = 0;
                k1 = 1;
                i2 = 1;
                j2 = 0;
                k2 = 1;
            }
        }
        else
        {
            // x0 < y0
            if (y0 < z0)
            {
                // Z Y X order
                i1 = 0;
                j1 = 0;
                k1 = 1;
                i2 = 0;
                j2 = 1;
                k2 = 1;
            }
            else if (x0 < z0)
            {
                // Y Z X order
                i1 = 0;
                j1 = 1;
                k1 = 0;
                i2 = 0;
                j2 = 1;
                k2 = 1;
            }
            else
            {
                // Y X Z order
                i1 = 0;
                j1 = 1;
                k1 = 0;
                i2 = 1;
                j2 = 1;
                k2 = 0;
            }
        }

        // A step of (1,0,0) in (i,j,k) means a step of (1-c,-c,-c) in (x,y,z),
        // a step of (0,1,0) in (i,j,k) means a step of (-c,1-c,-c) in (x,y,z), and
        // a step of (0,0,1) in (i,j,k) means a step of (-c,-c,1-c) in (x,y,z), where
        // c = 1/6.
        float x1 = x0 - i1 + G3; // Offsets for second corner in (x,y,z) coords
        float y1 = y0 - j1 + G3;
        float z1 = z0 - k1 + G3;
        float x2 = x0 - i2 + (2 * G3); // Offsets for third corner in (x,y,z) coords
        float y2 = y0 - j2 + (2 * G3);
        float z2 = z0 - k2 + (2 * G3);
        float x3 = x0 - 1 + (3 * G3); // Offsets for last corner in (x,y,z) coords
        float y3 = y0 - 1 + (3 * G3);
        float z3 = z0 - 1 + (3 * G3);

        // Wrap the integer indices at 256, to avoid indexing perm[] out of bounds
        int ii = i % 256;
        int jj = j % 256;
        int kk = k % 256;

        // Calculate the contribution from the four corners
        float t0 = 0.6f - (x0 * x0) - (y0 * y0) - (z0 * z0);
        if (t0 < 0)
        {
            n0 = 0;
        }
        else
        {
            t0 *= t0;
            n0 = t0 * t0 * GetGradient(perm[ii + perm[jj + perm[kk]]], x0, y0, z0);
        }

        float t1 = 0.6f - (x1 * x1) - (y1 * y1) - (z1 * z1);
        if (t1 < 0)
        {
            n1 = 0;
        }
        else
        {
            t1 *= t1;
            n1 = t1 * t1 * GetGradient(perm[ii + i1 + perm[jj + j1 + perm[kk + k1]]], x1, y1, z1);
        }

        float t2 = 0.6f - (x2 * x2) - (y2 * y2) - (z2 * z2);
        if (t2 < 0)
        {
            n2 = 0;
        }
        else
        {
            t2 *= t2;
            n2 = t2 * t2 * GetGradient(perm[ii + i2 + perm[jj + j2 + perm[kk + k2]]], x2, y2, z2);
        }

        float t3 = 0.6f - (x3 * x3) - (y3 * y3) - (z3 * z3);
        if (t3 < 0)
        {
            n3 = 0;
        }
        else
        {
            t3 *= t3;
            n3 = t3 * t3 * GetGradient(perm[ii + 1 + perm[jj + 1 + perm[kk + 1]]], x3, y3, z3);
        }

        // Add contributions from each corner to get the final noise value.
        // The result is scaled to stay just inside [-1,1]
        return 32 * (n0 + n1 + n2 + n3); // TODO: The scale factor is preliminary!
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// A fast method for obtaining the floored int value of a float.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The floored value.</returns>
    private static int FastFloor(float value)
    {
        return value > 0 ? (int)value : ((int)value - 1);
    }

    /// <summary>
    /// Gets a gradient value for the given hash code and x offset.
    /// </summary>
    /// <param name="hash">The hash code.</param>
    /// <param name="x">The x offset.</param>
    /// <returns>The gradient.</returns>
    private static float GetGradient(int hash, float x)
    {
        int h = hash & 15;

        // Gradient value 0 - 8
        float grad = h & 7;

        // Set a random sign for the gradient (50/50)
        if ((h & 8) != 0)
        {
            grad = -grad;
        }

        // Multiply the gradient with the distance
        return grad * x;
    }

    /// <summary>
    /// Gets a gradient value for the given hash code and x/y offsets.
    /// </summary>
    /// <param name="hash">The hash code.</param>
    /// <param name="x">The x offset.</param>
    /// <param name="y">The y offset.</param>
    /// <returns>The gradient.</returns>
    private static float GetGradient(int hash, float x, float y)
    {
        int h = hash & 7;

        // Convert low 3 bits of hash code into 8 gradient directions
        float u = h < 4 ? x : y;
        u = (h & 1) != 0 ? -u : u;
        float v = h < 4 ? y : x;
        v = (h & 2) != 0 ? -v : v;

        return u + (2 * v);
    }

    /// <summary>
    /// Gets a gradient value for the given hash code and x/y/z offsets.
    /// </summary>
    /// <param name="hash">The hash code.</param>
    /// <param name="x">The x offset.</param>
    /// <param name="y">The y offset.</param>
    /// <param name="z">The z offset.</param>
    /// <returns>The gradient.</returns>
    private static float GetGradient(int hash, float x, float y, float z)
    {
        int h = hash & 15;

        // Convert low 4 bits of hash code into 12 simple gradient directions
        float u = h < 8 ? x : y;
        u = (h & 1) != 0 ? -u : u;
        float v = h < 4 ? y : h == 12 || h == 14 ? x : z; // Fix repeats at h = 12 to 15
        v = (h & 2) != 0 ? -v : v;

        return u + v;
    }

    /// <summary>
    /// Gets a gradient value for the given hash code and x/y/z/t offsets.
    /// </summary>
    /// <param name="hash">The hash code.</param>
    /// <param name="x">The x offset.</param>
    /// <param name="y">The y offset.</param>
    /// <param name="z">The z offset.</param>
    /// <param name="t">The t offset.</param>
    /// <returns>The gradient.</returns>
    private static float GetGradient(int hash, float x, float y, float z, float t)
    {
        int h = hash & 31;

        // Convert low 5 bits of hash code into 32 simple gradient directions
        float u = h < 24 ? x : y;
        u = (h & 1) != 0 ? -u : u;
        float v = h < 16 ? y : z;
        v = (h & 2) != 0 ? -v : v;
        float w = h < 8 ? z : t;
        w = (h & 4) != 0 ? -w : w;

        return u + v + w;
    }

    #endregion
}