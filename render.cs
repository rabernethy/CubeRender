using System;
using System.IO;
using System.Text;


public class Renderer
{
	public static char intToBraille(int bits)
	{
		//Converts an int to the associated binary braille
		return Convert.ToChar(10240 + bits);
	}

	public static int[,] lineLow(int[,] screen, int x0, int y0, int x1, int y1)
	{
		// They go high, we go low
		int dx = x1 - x0;
		int dy = y1 - y0;
		int yi = 1;
		if (dy < 0) {
			yi = -1;
			dy = -dy;
		}
		int D = 2*dy - dx;
		int y = y0;
		for (int i=x0;i<=x1;i++){
			screen[i,y] = 1;
			if (D>0) {
				y += yi;
				D -= 2*dx;
			}
			D += 2*dy;
		}
		return screen;
	}

	public static int[,] lineHigh(int[,] screen, int x0, int y0, int x1, int y1)
	{
		// They go low, we go high
		int dx = x1 - x0;
		int dy = y1 - y0;
		int xi = 1;
		if (dx < 0) {
			xi = -1;
			dx = -dx;
		}
		int D = 2*dx - dy;
		int x = x0;
		for (int y=y0;y<=y1;y++){
			screen[x,y] = 1;
			if (D>0) {
				x += xi;
				D -= 2*dy;
			}
			D += 2*dx;
		}
		return screen;
	}

	public static int[,] line(ref int[,] screen, int x0, int y0, int x1, int y1)
	{
		if (Math.Abs(y1 - y0) < Math.Abs(x1 - x0)) {
			if (x0 > x1)
				return lineLow(screen, x1, y1, x0, y0);
			else 
				return lineLow(screen, x0, y0, x1, y1);
		} else {
			if (y0 > y1)
				return lineHigh(screen, x1, y1, x0, y0);
			else
				return lineHigh(screen, x0, y0, x1, y1);
		}
	}

	public static bool bounds(int[,] screen, int[] node) {
		return screen.GetLength(0) > node[0] && node[0] >= 0 && screen.GetLength(1) > node[1] && node[1] >= 0;
	}

	//returns a modified screen
	public static int[,] draw(ref int[,] screen, int[][] nodes, int[][] edges)
	{
		for (int e=0; e<edges.Length; e++) {
			int n0 = edges[e][0];
			int n1 = edges[e][1];
			int[] node0 = nodes[n0];
			int[] node1 = nodes[n1];
			if (!(bounds(screen, node0) && bounds(screen, node1)))
				continue;
			screen = line(ref screen, node0[0], node0[1], node1[0], node1[1]);
		}
		foreach (int[] node in nodes) {
			if (!bounds(screen, node))
				continue;
			screen[node[0], node[1]] = 1;
		}
		return screen;
	}

	//returns a cleared screen. Should be in a class
	public static int[,] clear(int[,] screen) {
		for (int i=0;i<screen.GetLength(0);i++){
			for(int j=0;j<screen.GetLength(1);j++){
				screen[i,j] = 0;
			}
		}
		return screen;
	}
	
	// Blits Information to the screen
	public static void blit(int[,] screen)
	{
		for (int j=0; j<screen.GetLength(1)/4; j++){
		for (int i=0; i<screen.GetLength(0)/2; i++) {
			string bin = "" + 
				screen[i*2+1,j*4+3].ToString() +
				screen[i*2,j*4+3].ToString() +
				screen[i*2+1,j*4+2].ToString() +
				screen[i*2+1,j*4+1].ToString() +
				screen[i*2+1,j*4].ToString() +
				screen[i*2,j*4+2].ToString() +
				screen[i*2,j*4+1].ToString() +
				screen[i*2,j*4].ToString();
			//Console.Write(bin);
			Console.Write(intToBraille(Convert.ToInt32(bin, 2)));
		}
			Console.WriteLine();
		}
	}

	//Rotate along the Z axis
	public static int[][] rotateZ3D(ref int[][] nodes, double theta) {
		for (int i=0; i<nodes.Length; i++){
			int x = nodes[i][0];
			int y = nodes[i][1];
			nodes[i][0] = (int) (x * Math.Cos(theta) - y * Math.Sin(theta));
			nodes[i][1] = (int) (y * Math.Cos(theta) + x * Math.Sin(theta));
		}
		return nodes;
	}

	public static void Main(string[] args)
	{
		int[] resolution = {32, 32};
		int[,] screen = new int[resolution[0], resolution[1]];
		screen = clear(screen);
		int[][] nodes = 
		{
			new int[] {1, 1, 1},
			new int[] {1,1, 20},
			new int[] {1,20, 1},
			new int[] {1, 20, 20},
			new int[] {20, 1, 1},
			new int[] {20, 1, 20},
			new int[] {20,20,1},
			new int[] {20,20,20}
		};
		int[][] edges = 
		{
			new int[] {0,1},
			new int[] {1,3},
			new int[] {3,2},
			new int[] {2,0},
			new int[] {4,5},
			new int[] {5,7},
			new int[] {7,6},
			new int[] {6,4},
			new int[] {0,4},
			new int[] {1,5},
			new int[] {2,6},
			new int[] {3,7}
		};

		
		for (int i=0;i<10;i++) {
			rotateZ3D(ref nodes, Math.PI/4);
			draw(ref screen, nodes, edges);
			blit(screen);
			Console.WriteLine();
		}
	}
}
