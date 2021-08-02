using System;
using System.Collections.Generic;
using System.Linq;

using Landwerth.ProgramAnalysis;

/**
*   import java.util.Scanner;

class Zigzag {
	
	void get() {
		Scanner sc = new Scanner(System.in);
		int r, c;
		do {
			System.out.println("Enter row & col");
			r = sc.nextInt();
			c = sc.nextInt();
		} while (r <= 0 || c <= 0);
		
		int x[][] = new int[r][c];
		
		fill(x);
		display(x);
	}
	
	void fill(int Q[][]) {
		int z = 1;
		for(int i = 0; i < Q.length; i++) {
			for(int j = 0; j < Q[0].length; j++, z++) {
				if(i % 2 == 0)
					Q[i][j] = z;
				else
					Q[i][Q[0].length - j - 1] = z;
			}
		}
	}
	
	void display(int x[][]) {
		System.out.println();
		for(int h[] : x) {
			for(int g : h) {
				System.out.print(g + "\t");
			}
			System.out.println();
		}
	}
	
	public static void main(String args[]) {
		new Zigzag().get();
	}
}
*/

/**
import java.util.Scanner;

class RotateMatrix {
	
	int[][] transpose(int[][] A, int n) {
		int[][] B = new int[n][n];
		for(int i = 0; i < n; i++) {
			for(int j = 0; j < n; j++) {
				B[i][j] = A[j][i];
			}
		}
		return B;	
	}

	int[][] rotate90(int[][] A, int n) {
		int[][] B = new int[n][n];
		A = transpose(A, n);
		for(int i = 0; i < n; i++) {
			for(int j = 0; j < n; j++) {
				B[i][j] = A[i][n-1-j];
			}
		}
		return B;	
	}

	int[][] rotate180(int[][] A, int n) {
		return rotate90(rotate90(A, n), n);
	}

	public static void main(String args[]) {
		Scanner sc = new Scanner(System.in);
		int n = sc.nextInt();
		int a[][] = new int[n][n];
		for(int i = 0; i < n; i++) { 
			for(int j = 0; j < n; j++) {
				System.out.println("Enter element at (" + i + ", "  + j + ") :");
				a[i][j] = sc.nextInt();
			}
		}
		
		int[][] matrix = new int[n][n];
		
		for(int i = 0; i < n; i++) {
			for(int j = 0; j < n; j++) {
				matrix[i][j] = a[i][j];
			}
		}
		
		for(int i = 0; i < n; i++) {
			for(int j = 0; j < n; j++) {
				System.out.print(matrix[i][j] + " ");
			}
			System.out.println();
		}
		
		System.out.println("90 degrees rotation: ");

		a = new RotateMatrix().rotate90(matrix, n);		
		for(int i = 0; i < n; i++) {
			for(int j = 0; j < n; j++) {
				System.out.print(a[i][j] + " ");
			}
			System.out.println();
		}

		System.out.println("180 degrees rotation: ");
	
		matrix = new RotateMatrix().rotate180(matrix, n);		
		for(int i = 0; i < n; i++) {
			for(int j = 0; j < n; j++) {
				System.out.print(matrix[i][j] + " ");
			}
			System.out.println();
		}
	}
}

**/

namespace Landwerth
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Landwerth Compiler, Version 0.0.0.1");
            bool showTree = false;
            while(true) 
            {
                Console.Write("lc> ");
                var line = Console.ReadLine();
                
                if(string.IsNullOrWhiteSpace(line))
                    return;
                
                if(line == "#showTree") 
                {
                    showTree = !showTree;
                    Console.WriteLine((showTree) ? "Parse tree viewing enabled." : "Parse tree viewing disabled.");
                    continue;
                }
                else if((line == "#clear") || (line == "#cls"))
                {
                    Console.Clear();
                    continue;
                }


                var syntaxTree = SyntaxTree.Parse(line);

                if(showTree) 
                {
                    var colour = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Display(syntaxTree.Root);
                    Console.ForegroundColor = colour;
                }

                if (!syntaxTree.Diagnostics.Any())
                {
                    var e = new Evaluator(syntaxTree.Root);
                    var result = e.Evaluate();
                    Console.WriteLine(result);
                }
                else
                {
                    var colour = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;

                    foreach (var diagnostics in syntaxTree.Diagnostics)
                        Console.WriteLine(diagnostics);

                    Console.ForegroundColor = colour;
                }
            }
        }

        static void Display(SyntaxNode node, String indent = "", bool isLast = true)
        {
            
            var marker = (isLast) ? "└──" : "├──";

            Console.Write(indent);
            Console.Write(marker);
            Console.Write(node.Type);

            if((node is SyntaxToken t) && (t.Value != null))
            {
                Console.Write(" ");
                Console.Write(t.Value);
            }

            Console.WriteLine();

            indent += (isLast) ? "\t" : "│   ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
                Display(child, indent, child == lastChild);
        }
    }

}
