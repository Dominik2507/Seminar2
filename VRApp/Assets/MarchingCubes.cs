using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingCubes : MonoBehaviour
{

    public static int GetCubeConfig(short[,,] grid, int x, int y, int z)
    {
        int a = grid[x, y, z];
        int b = grid[x, y, z + 1];
        int c = grid[x, y + 1, z + 1];
        int d = grid[x, y + 1, z];
        int e = grid[x + 1, y, z];
        int f = grid[x + 1, y, z + 1];
        int g = grid[x + 1, y + 1, z + 1];
        int h = grid[x + 1, y + 1, z];

        int sum = a + b + c + d + e + f + g + h;

        // NO POINTS (inside or outside of the object)
        //256 = 254 combinations with faces and 2 with no faces
        if (sum == 0 || sum == 8) return 0;
        

        #region 1 point configurations
        if (a == 1 && (a + b + c + d + e + f + g + h == 1)) return 7;
        if (a == 0 && (a + b + c + d + e + f + g + h == 7)) return 8;
        if (b == 1 && (a + b + c + d + e + f + g + h == 1)) return 9; 
        if (b == 0 && (a + b + c + d + e + f + g + h == 7)) return 10;
        if (c == 1 && (a + b + c + d + e + f + g + h == 1)) return 11;
        if (c == 0 && (a + b + c + d + e + f + g + h == 7)) return 12;
        if (d == 1 && (a + b + c + d + e + f + g + h == 1)) return 13;
        if (d == 0 && (a + b + c + d + e + f + g + h == 7)) return 14;
        if (e == 1 && (a + b + c + d + e + f + g + h == 1)) return 15;
        if (e == 0 && (a + b + c + d + e + f + g + h == 7)) return 16;
        if (f == 1 && (a + b + c + d + e + f + g + h == 1)) return 17;
        if (f == 0 && (a + b + c + d + e + f + g + h == 7)) return 18;
        if (g == 1 && (a + b + c + d + e + f + g + h == 1)) return 19;
        if (g == 0 && (a + b + c + d + e + f + g + h == 7)) return 20;
        if (h == 1 && (a + b + c + d + e + f + g + h == 1)) return 21;
        if (h == 0 && (a + b + c + d + e + f + g + h == 7)) return 22;
        #endregion


        #region 2 points configurations
        
        #region 2 point on one egde (in or out)
        if ((a + b == 2) && (a + b + c + d + e + f + g + h == 2)) return 23;
        if ((a + b == 0) && (a + b + c + d + e + f + g + h == 6)) return 24;
        if ((a + d == 2) && (a + b + c + d + e + f + g + h == 2)) return 25;
        if ((a + d == 0) && (a + b + c + d + e + f + g + h == 6)) return 26;
        if ((a + e == 2) && (a + b + c + d + e + f + g + h == 2)) return 27;
        if ((a + e == 0) && (a + b + c + d + e + f + g + h == 6)) return 28;

        
        if ((g + h == 2) && (a + b + c + d + e + f + g + h == 2)) return 29;
        if ((g + h == 0) && (a + b + c + d + e + f + g + h == 6)) return 30;
        if ((g + f == 2) && (a + b + c + d + e + f + g + h == 2)) return 31;
        if ((g + f == 0) && (a + b + c + d + e + f + g + h == 6)) return 32;
        if ((g + c == 2) && (a + b + c + d + e + f + g + h == 2)) return 33;
        if ((g + c == 0) && (a + b + c + d + e + f + g + h == 6)) return 34;

        if ((b + c == 2) && (a + b + c + d + e + f + g + h == 2)) return 35;
        if ((b + c == 0) && (a + b + c + d + e + f + g + h == 6)) return 36;
        if ((c + d == 2) && (a + b + c + d + e + f + g + h == 2)) return 37;
        if ((c + d == 0) && (a + b + c + d + e + f + g + h == 6)) return 38;
        if ((b + f == 2) && (a + b + c + d + e + f + g + h == 2)) return 39;
        if ((b + f == 0) && (a + b + c + d + e + f + g + h == 6)) return 40;

        if ((e + f == 2) && (a + b + c + d + e + f + g + h == 2)) return 41;
        if ((e + f == 0) && (a + b + c + d + e + f + g + h == 6)) return 42;
        if ((e + h == 2) && (a + b + c + d + e + f + g + h == 2)) return 43;
        if ((e + h == 0) && (a + b + c + d + e + f + g + h == 6)) return 44;
        if ((d + h == 2) && (a + b + c + d + e + f + g + h == 2)) return 45;
        if ((d + h == 0) && (a + b + c + d + e + f + g + h == 6)) return 46;
        #endregion
        
        #region 2 on face diagonal (in or out)
        if ((a + c == 2) && (a + b + c + d + e + f + g + h == 2)) return 47;
        if ((a + c == 0) && (a + b + c + d + e + f + g + h == 6)) return 48;
        if ((b + d == 2) && (a + b + c + d + e + f + g + h == 2)) return 49;
        if ((b + d == 0) && (a + b + c + d + e + f + g + h == 6)) return 50;
        if ((e + g == 2) && (a + b + c + d + e + f + g + h == 2)) return 51;
        if ((e + g == 0) && (a + b + c + d + e + f + g + h == 6)) return 52;
        if ((f + h == 2) && (a + b + c + d + e + f + g + h == 2)) return 53;
        if ((f + h == 0) && (a + b + c + d + e + f + g + h == 6)) return 54;
        if ((a + f == 2) && (a + b + c + d + e + f + g + h == 2)) return 55;
        if ((a + f == 0) && (a + b + c + d + e + f + g + h == 6)) return 56;
        if ((b + e == 2) && (a + b + c + d + e + f + g + h == 2)) return 57;
        if ((b + e == 0) && (a + b + c + d + e + f + g + h == 6)) return 58;
        if ((b + g == 2) && (a + b + c + d + e + f + g + h == 2)) return 59;
        if ((b + g == 0) && (a + b + c + d + e + f + g + h == 6)) return 60;
        if ((c + f == 2) && (a + b + c + d + e + f + g + h == 2)) return 61;
        if ((c + f == 0) && (a + b + c + d + e + f + g + h == 6)) return 62;
        if ((c + h == 2) && (a + b + c + d + e + f + g + h == 2)) return 63;
        if ((c + h == 0) && (a + b + c + d + e + f + g + h == 6)) return 64;
        if ((d + g == 2) && (a + b + c + d + e + f + g + h == 2)) return 65;
        if ((d + g == 0) && (a + b + c + d + e + f + g + h == 6)) return 66;
        if ((a + h == 2) && (a + b + c + d + e + f + g + h == 2)) return 67;
        if ((a + h == 0) && (a + b + c + d + e + f + g + h == 6)) return 68;
        if ((d + e == 2) && (a + b + c + d + e + f + g + h == 2)) return 69;
        if ((d + e == 0) && (a + b + c + d + e + f + g + h == 6)) return 70;
        #endregion

        #region 2 on space diagonal
        if ((a + g == 2) && (a + b + c + d + e + f + g + h == 2)) return 71;
        if ((a + g == 0) && (a + b + c + d + e + f + g + h == 6)) return 72;
        if ((b + h == 2) && (a + b + c + d + e + f + g + h == 2)) return 73;
        if ((b + h == 0) && (a + b + c + d + e + f + g + h == 6)) return 74;
        if ((c + e == 2) && (a + b + c + d + e + f + g + h == 2)) return 75;
        if ((c + e == 0) && (a + b + c + d + e + f + g + h == 6)) return 76;
        if ((d + f == 2) && (a + b + c + d + e + f + g + h == 2)) return 77;
        if ((d + f == 0) && (a + b + c + d + e + f + g + h == 6)) return 78;
        #endregion
        
        #endregion

        
        #region 3 points configurations
        
        #region 3 points in, 3 on same face
        if ((a + b + c == 3) && (a + b + c + d + e + f + g + h == 3)) return 79;
        if ((a + b + c == 0) && (a + b + c + d + e + f + g + h == 5)) return 80;
        if ((a + b + d == 3) && (a + b + c + d + e + f + g + h == 3)) return 81;
        if ((a + b + d == 0) && (a + b + c + d + e + f + g + h == 5)) return 82;
        if ((a + c + d == 3) && (a + b + c + d + e + f + g + h == 3)) return 83;
        if ((a + c + d == 0) && (a + b + c + d + e + f + g + h == 5)) return 84;
        if ((b + c + d == 3) && (a + b + c + d + e + f + g + h == 3)) return 85;
        if ((b + c + d == 0) && (a + b + c + d + e + f + g + h == 5)) return 86;
        
        if ((e + f + g == 3) && (a + b + c + d + e + f + g + h == 3)) return 87;
        if ((e + f + g == 0) && (a + b + c + d + e + f + g + h == 5)) return 88;
        if ((e + f + h == 3) && (a + b + c + d + e + f + g + h == 3)) return 89;
        if ((e + f + h == 0) && (a + b + c + d + e + f + g + h == 5)) return 90;
        if ((e + g + h == 3) && (a + b + c + d + e + f + g + h == 3)) return 91;
        if ((e + g + h == 0) && (a + b + c + d + e + f + g + h == 5)) return 92;
        if ((f + g + h == 3) && (a + b + c + d + e + f + g + h == 3)) return 93;
        if ((f + g + h == 0) && (a + b + c + d + e + f + g + h == 5)) return 94;

        if ((a + b + f == 3) && (a + b + c + d + e + f + g + h == 3)) return 95;
        if ((a + b + f == 0) && (a + b + c + d + e + f + g + h == 5)) return 96;
        if ((a + e + f == 3) && (a + b + c + d + e + f + g + h == 3)) return 97;
        if ((a + e + f == 0) && (a + b + c + d + e + f + g + h == 5)) return 98;
        if ((a + b + e == 3) && (a + b + c + d + e + f + g + h == 3)) return 99;
        if ((a + b + e == 0) && (a + b + c + d + e + f + g + h == 5)) return 100;
        if ((b + e + f == 3) && (a + b + c + d + e + f + g + h == 3)) return 101;
        if ((b + e + f == 0) && (a + b + c + d + e + f + g + h == 5)) return 102;

        if ((b + c + g == 3) && (a + b + c + d + e + f + g + h == 3)) return 103;
        if ((b + c + g == 0) && (a + b + c + d + e + f + g + h == 5)) return 104;
        if ((b + f + g == 3) && (a + b + c + d + e + f + g + h == 3)) return 105;
        if ((b + f + g == 0) && (a + b + c + d + e + f + g + h == 5)) return 106;
        if ((b + f + c == 3) && (a + b + c + d + e + f + g + h == 3)) return 107;
        if ((b + f + c == 0) && (a + b + c + d + e + f + g + h == 5)) return 108;
        if ((c + f + g == 3) && (a + b + c + d + e + f + g + h == 3)) return 109;
        if ((c + f + g == 0) && (a + b + c + d + e + f + g + h == 5)) return 110;

        if ((c + g + h == 3) && (a + b + c + d + e + f + g + h == 3)) return 111;
        if ((c + g + h == 0) && (a + b + c + d + e + f + g + h == 5)) return 112;
        if ((c + g + d == 3) && (a + b + c + d + e + f + g + h == 3)) return 113;
        if ((c + g + d == 0) && (a + b + c + d + e + f + g + h == 5)) return 114;
        if ((c + d + h == 3) && (a + b + c + d + e + f + g + h == 3)) return 115;
        if ((c + d + h == 0) && (a + b + c + d + e + f + g + h == 5)) return 116;
        if ((d + g + h == 3) && (a + b + c + d + e + f + g + h == 3)) return 117;
        if ((d + g + h == 0) && (a + b + c + d + e + f + g + h == 5)) return 118;

        if ((a + d + h == 3) && (a + b + c + d + e + f + g + h == 3)) return 119;
        if ((a + d + h == 0) && (a + b + c + d + e + f + g + h == 5)) return 120;
        if ((a + d + e == 3) && (a + b + c + d + e + f + g + h == 3)) return 121;
        if ((a + d + e == 0) && (a + b + c + d + e + f + g + h == 5)) return 122;
        if ((a + e + h == 3) && (a + b + c + d + e + f + g + h == 3)) return 123;
        if ((a + e + h == 0) && (a + b + c + d + e + f + g + h == 5)) return 124;
        if ((d + e + h == 3) && (a + b + c + d + e + f + g + h == 3)) return 125;
        if ((d + e + h == 0) && (a + b + c + d + e + f + g + h == 5)) return 126;
        #endregion
        
        #region 3 in, 2 on edge, 1 diagonally

        if ((a + b + g == 3) && (a + b + c + d + e + f + g + h == 3)) return 127;
        if ((a + b + g == 0) && (a + b + c + d + e + f + g + h == 5)) return 128;
        if ((a + b + h == 3) && (a + b + c + d + e + f + g + h == 3)) return 129;
        if ((a + b + h == 0) && (a + b + c + d + e + f + g + h == 5)) return 130;

        if ((b + c + e == 3) && (a + b + c + d + e + f + g + h == 3)) return 131;
        if ((b + c + e == 0) && (a + b + c + d + e + f + g + h == 5)) return 132;
        if ((b + c + h == 3) && (a + b + c + d + e + f + g + h == 3)) return 133;
        if ((b + c + h == 0) && (a + b + c + d + e + f + g + h == 5)) return 134;

        if ((c + d + e == 3) && (a + b + c + d + e + f + g + h == 3)) return 135;
        if ((c + d + e == 0) && (a + b + c + d + e + f + g + h == 5)) return 136;
        if ((c + d + f == 3) && (a + b + c + d + e + f + g + h == 3)) return 137;
        if ((c + d + f == 0) && (a + b + c + d + e + f + g + h == 5)) return 138;

        if ((d + a + g == 3) && (a + b + c + d + e + f + g + h == 3)) return 139;
        if ((d + a + g == 0) && (a + b + c + d + e + f + g + h == 5)) return 140;
        if ((d + a + f == 3) && (a + b + c + d + e + f + g + h == 3)) return 141;
        if ((d + a + f == 0) && (a + b + c + d + e + f + g + h == 5)) return 142;

        if ((e + f + c == 3) && (a + b + c + d + e + f + g + h == 3)) return 143;
        if ((e + f + c == 0) && (a + b + c + d + e + f + g + h == 5)) return 144;
        if ((e + f + d == 3) && (a + b + c + d + e + f + g + h == 3)) return 145;
        if ((e + f + d == 0) && (a + b + c + d + e + f + g + h == 5)) return 146;
        
        if ((f + g + d == 3) && (a + b + c + d + e + f + g + h == 3)) return 147;
        if ((f + g + d == 0) && (a + b + c + d + e + f + g + h == 5)) return 148;
        if ((f + g + a == 3) && (a + b + c + d + e + f + g + h == 3)) return 149;
        if ((f + g + a == 0) && (a + b + c + d + e + f + g + h == 5)) return 150;

        if ((g + h + a == 3) && (a + b + c + d + e + f + g + h == 3)) return 151;
        if ((g + h + a == 0) && (a + b + c + d + e + f + g + h == 5)) return 152;
        if ((g + h + b == 3) && (a + b + c + d + e + f + g + h == 3)) return 153;
        if ((g + h + b == 0) && (a + b + c + d + e + f + g + h == 5)) return 154;
        
        if ((e + h + c == 3) && (a + b + c + d + e + f + g + h == 3)) return 155;
        if ((e + h + c == 0) && (a + b + c + d + e + f + g + h == 5)) return 156;
        if ((e + h + b == 3) && (a + b + c + d + e + f + g + h == 3)) return 157;
        if ((e + h + b == 0) && (a + b + c + d + e + f + g + h == 5)) return 158;

        if ((a + e + c == 3) && (a + b + c + d + e + f + g + h == 3)) return 159;
        if ((a + e + c == 0) && (a + b + c + d + e + f + g + h == 5)) return 160;
        if ((a + e + g == 3) && (a + b + c + d + e + f + g + h == 3)) return 161;
        if ((a + e + g == 0) && (a + b + c + d + e + f + g + h == 5)) return 162;
        
        if ((b + f + d == 3) && (a + b + c + d + e + f + g + h == 3)) return 163;
        if ((b + f + d == 0) && (a + b + c + d + e + f + g + h == 5)) return 164;
        if ((b + f + h == 3) && (a + b + c + d + e + f + g + h == 3)) return 165;
        if ((b + f + h == 0) && (a + b + c + d + e + f + g + h == 5)) return 166;

        if ((c + g + a == 3) && (a + b + c + d + e + f + g + h == 3)) return 167;
        if ((c + g + a == 0) && (a + b + c + d + e + f + g + h == 5)) return 168;
        if ((c + g + e == 3) && (a + b + c + d + e + f + g + h == 3)) return 169;
        if ((c + g + e == 0) && (a + b + c + d + e + f + g + h == 5)) return 170;
        
        if ((d + h + b == 3) && (a + b + c + d + e + f + g + h == 3)) return 171;
        if ((d + h + b == 0) && (a + b + c + d + e + f + g + h == 5)) return 172;
        if ((d + h + f == 3) && (a + b + c + d + e + f + g + h == 3)) return 173;
        if ((d + h + f == 0) && (a + b + c + d + e + f + g + h == 5)) return 174;
        #endregion

        #region 3 in, 3 not connected
        if ((a + f + h == 3) && (a + b + c + d + e + f + g + h == 3)) return 175;
        if ((a + f + h == 0) && (a + b + c + d + e + f + g + h == 5)) return 176;

        if ((a + c + f == 3) && (a + b + c + d + e + f + g + h == 3)) return 177;
        if ((a + c + f == 0) && (a + b + c + d + e + f + g + h == 5)) return 178;

        if ((a + c + h == 3) && (a + b + c + d + e + f + g + h == 3)) return 179;
        if ((a + c + h == 0) && (a + b + c + d + e + f + g + h == 5)) return 180;

        if ((b + g + e == 3) && (a + b + c + d + e + f + g + h == 3)) return 181;
        if ((b + g + e == 0) && (a + b + c + d + e + f + g + h == 5)) return 182;

        if ((b + d + e == 3) && (a + b + c + d + e + f + g + h == 3)) return 183;
        if ((b + d + e == 0) && (a + b + c + d + e + f + g + h == 5)) return 184;

        if ((b + d + g == 3) && (a + b + c + d + e + f + g + h == 3)) return 185;
        if ((b + d + g == 0) && (a + b + c + d + e + f + g + h == 5)) return 186;

        if ((c + f + h == 3) && (a + b + c + d + e + f + g + h == 3)) return 187;
        if ((c + f + h == 0) && (a + b + c + d + e + f + g + h == 5)) return 188;

        if ((d + e + g == 3) && (a + b + c + d + e + f + g + h == 3)) return 189;
        if ((d + e + g == 0) && (a + b + c + d + e + f + g + h == 5)) return 190;


        #endregion

        #endregion
        
        
        #region 4 points configurations

        #region 4 points on one face
        if ((a + b + c + d == 4) && (e + f + g + h == 0)) return 1;
        if ((a + b + c + d == 0) && (e + f + g + h == 4)) return 2;
        if ((a + b + e + f == 4) && (c + d + g + h == 0)) return 3;
        if ((a + b + e + f == 0) && (c + d + g + h == 4)) return 4;
        if ((a + d + e + h == 4) && (b + c + f + g == 0)) return 5;
        if ((a + d + e + h == 0) && (b + c + f + g == 4)) return 6;
        #endregion
        
        #region 4 points in, 3 on face same, 1 not connected to any
        if ((a + b + c + h == 4) && (a + b + c + d + e + f + g + h == 4)) return 191;
        if ((a + b + c + h == 0) && (a + b + c + d + e + f + g + h == 4)) return 192;
        if ((a + b + g + d == 4) && (a + b + c + d + e + f + g + h == 4)) return 193;
        if ((a + b + g + d == 0) && (a + b + c + d + e + f + g + h == 4)) return 194;
        if ((a + f + c + d == 4) && (a + b + c + d + e + f + g + h == 4)) return 195;
        if ((a + f + c + d == 0) && (a + b + c + d + e + f + g + h == 4)) return 196;
        if ((e + b + c + d == 4) && (a + b + c + d + e + f + g + h == 4)) return 197;
        if ((e + b + c + d == 0) && (a + b + c + d + e + f + g + h == 4)) return 198;
       
        if ((a + b + f + h == 4) && (a + b + c + d + e + f + g + h == 4)) return 199;
        if ((a + b + f + h == 0) && (a + b + c + d + e + f + g + h == 4)) return 200;
        if ((a + b + g + e == 4) && (a + b + c + d + e + f + g + h == 4)) return 201;
        if ((a + b + g + e == 0) && (a + b + c + d + e + f + g + h == 4)) return 202;
        if ((a + c + f + e == 4) && (a + b + c + d + e + f + g + h == 4)) return 203;
        if ((a + c + f + e == 0) && (a + b + c + d + e + f + g + h == 4)) return 204;
        if ((d + b + f + e == 4) && (a + b + c + d + e + f + g + h == 4)) return 205;
        if ((d + b + f + e == 0) && (a + b + c + d + e + f + g + h == 4)) return 206;

        if ((b + c + g + e == 4) && (a + b + c + d + e + f + g + h == 4)) return 207;
        if ((b + c + g + e == 0) && (a + b + c + d + e + f + g + h == 4)) return 208;
        if ((b + c + h + f == 4) && (a + b + c + d + e + f + g + h == 4)) return 209;
        if ((b + c + h + f == 0) && (a + b + c + d + e + f + g + h == 4)) return 210;
        if ((b + d + g + f == 4) && (a + b + c + d + e + f + g + h == 4)) return 211;
        if ((b + d + g + f == 0) && (a + b + c + d + e + f + g + h == 4)) return 212;
        if ((a + c + g + f == 4) && (a + b + c + d + e + f + g + h == 4)) return 213;
        if ((a + c + g + f == 0) && (a + b + c + d + e + f + g + h == 4)) return 214;
        #endregion

        #region 4 points in, one connected to rest by edges
        if ((a + b + d + e == 4) && (a + b + c + d + e + f + g + h == 4)) return 215;
        if ((a + b + d + e == 0) && (a + b + c + d + e + f + g + h == 4)) return 216;
        if ((b + c + f + a == 4) && (a + b + c + d + e + f + g + h == 4)) return 217;
        if ((b + c + f + a == 0) && (a + b + c + d + e + f + g + h == 4)) return 218;
        if ((c + d + g + b == 4) && (a + b + c + d + e + f + g + h == 4)) return 219;
        if ((c + d + g + b == 0) && (a + b + c + d + e + f + g + h == 4)) return 220;
        if ((d + a + c + h == 4) && (a + b + c + d + e + f + g + h == 4)) return 221;
        if ((d + a + c + h == 0) && (a + b + c + d + e + f + g + h == 4)) return 222;
        #endregion

        #region 4 in, 3 on face, 1 connected to not the centre one
        if ((a + b + c + e == 4) && (a + b + c + d + e + f + g + h == 4)) return 223;
        if ((a + b + c + e == 0) && (a + b + c + d + e + f + g + h == 4)) return 224;
        if ((a + b + c + g == 4) && (a + b + c + d + e + f + g + h == 4)) return 225;
        if ((a + b + c + g == 0) && (a + b + c + d + e + f + g + h == 4)) return 226;
        if ((a + b + h + d == 4) && (a + b + c + d + e + f + g + h == 4)) return 227;
        if ((a + b + h + d == 0) && (a + b + c + d + e + f + g + h == 4)) return 228;
        if ((a + b + f + d == 4) && (a + b + c + d + e + f + g + h == 4)) return 229;
        if ((a + b + f + d == 0) && (a + b + c + d + e + f + g + h == 4)) return 230;
        if ((a + e + c + d == 4) && (a + b + c + d + e + f + g + h == 4)) return 231;
        if ((a + e + c + d == 0) && (a + b + c + d + e + f + g + h == 4)) return 232;
        if ((a + g + c + d == 4) && (a + b + c + d + e + f + g + h == 4)) return 233;
        if ((a + g + c + d == 0) && (a + b + c + d + e + f + g + h == 4)) return 234;

        if ((h + b + c + d == 4) && (a + b + c + d + e + f + g + h == 4)) return 235;
        if ((h + b + c + d == 0) && (a + b + c + d + e + f + g + h == 4)) return 236;

        if ((f + b + c + d == 4) && (a + b + c + d + e + f + g + h == 4)) return 237;
        if ((f + b + c + d == 0) && (a + b + c + d + e + f + g + h == 4)) return 238;

        if ((a + b + f + g == 4) && (a + b + c + d + e + f + g + h == 4)) return 239;
        if ((a + b + f + g == 0) && (a + b + c + d + e + f + g + h == 4)) return 240;
        
        if ((a + b + h + e == 4) && (a + b + c + d + e + f + g + h == 4)) return 241;
        if ((a + b + h + e == 0) && (a + b + c + d + e + f + g + h == 4)) return 242;
   
        if ((a + d + f + e == 4) && (a + b + c + d + e + f + g + h == 4)) return 243;
        if ((a + d + f + e == 0) && (a + b + c + d + e + f + g + h == 4)) return 244;

        if ((c + b + f + e == 4) && (a + b + c + d + e + f + g + h == 4)) return 245;
        if ((c + b + f + e == 0) && (a + b + c + d + e + f + g + h == 4)) return 246;

        #endregion

        #region 4 in, diagonals (2 diagonal edges)
        if ((a + e + c + g == 4) && (a + b + c + d + e + f + g + h == 4)) return 247;
        if ((a + e + c + g == 0) && (a + b + c + d + e + f + g + h == 4)) return 248;
        if ((a + b + g + h == 4) && (a + b + c + d + e + f + g + h == 4)) return 249;
        if ((a + b + g + h == 0) && (a + b + c + d + e + f + g + h == 4)) return 250;
        if ((a + d + f + g == 4) && (a + b + c + d + e + f + g + h == 4)) return 251;
        if ((a + d + f + g == 0) && (a + b + c + d + e + f + g + h == 4)) return 252;
        #endregion

        #region 4 in none connected
        if ((a + c + f + h == 4) && (a + b + c + d + e + f + g + h == 4)) return 253;
        if ((a + c + f + h == 0) && (a + b + c + d + e + f + g + h == 4)) return 254;

        
        #endregion
        
        #endregion

        return 0;
    }

    public static void AddMeshFaces()
    {

    }
}
