using System;

public class Class1
{
	public Class1()
	{
		public static void Main()
		{
			//9.a
			//int[] arr = new int[] { 1, 9, 6, 7, 5, 9 };

			//var arrAscending = arr.OrderBy(c => c).ToArray();
			//var arrDescending = arr.OrderByDescending(c => c).ToArray();

			//Console.Write("array Ascending is: ");
			//foreach (int value in arrAscending)
			//{
			//	Console.Write("value: " + value);
			//}

			//Console.Write("array Descending is: ");
			//foreach (int value in arrDescending)
			//{
			//	Console.Write("value:  " + value);
			//}

			//9.b
			int[] arrB = new int[10];
			for (int i = 0; i < 10; i++)
				arrB[i] = i + 1;

			int[] newarr = null;

			int x = 4;
			int pos = 5;
			for (var i = 0; i < arrB.Length; i++)
			{
				if (arrB[i] != x)
				{
					newarr[i] = arrB[i];
				}
			}

			foreach (int value in newarr)
			{
				Console.Write("value:  " + value);
			}
		}
	}
}