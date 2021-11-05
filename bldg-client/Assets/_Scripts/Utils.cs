using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
// using NUnit.Framework;


namespace Utils {
	public class AddressUtils {

		public static readonly string DELIM = "/";
		public static readonly char DELIM_CHAR = '/';
		public static readonly char[] DELIM_CHAR_ARRAY = { DELIM_CHAR };


		public static string extractFlr(string addr) {
			string[] parts = addr.Split (DELIM_CHAR_ARRAY);
			if (parts [parts.Length - 1].StartsWith ("b"))
				parts = parts.Take(parts.Length - 1).ToArray();
			return parts [parts.Length - 1];
		}



		public static string getContainerFlr(string addr) {
			string[] parts = addr.Split (DELIM_CHAR_ARRAY);
			if (parts.Length == 1)
				return parts [0];
			if (parts [parts.Length - 1].StartsWith ("b"))
				// remove just the building part
				parts = parts.Take(parts.Length - 1).ToArray();
			else
				// remove last 2 parts
				parts = parts.Take(parts.Length - 2).ToArray();
			return string.Join (DELIM, parts);
		}


		public static string getFlr(string addr) {
			// returns the current flr, i.e., if the addr is a bldg - removes the last part
			string[] parts = addr.Split (DELIM_CHAR_ARRAY);
			if (parts [parts.Length - 1].StartsWith ("b")) {
				parts = parts.Take(parts.Length - 1).ToArray();
				addr = string.Join (DELIM, parts);
			}
			return addr;
		}

		public static bool isBldg(string addr) {
			// returns indication whether the addr corresponds to a bldg
			string[] parts = addr.Split (DELIM_CHAR_ARRAY);
			return parts [parts.Length - 1].StartsWith ("b");
		}

		public static string generateInsideAddress(string addr) {
			if (isBldg (addr)) {
				addr = addr + DELIM_CHAR + "l0";
			}
			return addr;
		}

		public static string getBldg(string addr) {
			// returns the current bldg, i.e., if the addr is a flr - removes the last part
			string[] parts = addr.Split (DELIM_CHAR_ARRAY);
			if (parts [parts.Length - 1].StartsWith ("l")) {
				parts = parts.Take(parts.Length - 1).ToArray();
				addr = string.Join (DELIM, parts);
			}
			return addr;
		}

		public static int getFlrLevel(string flr_addr) {
			string[] parts = flr_addr.Split (DELIM_CHAR_ARRAY);
			string last_part = parts [parts.Length - 1];
			if (!last_part.StartsWith ("l")) {
				throw new System.ArgumentException ("Expected a floor address, but got: " + flr_addr);
			}
			string level_str = last_part.Substring (1);
			return System.Int32.Parse(level_str);
		}

		public static string updateLocation(string addr, int newX, int newY) {
			// replaces the coordinates in the container bldg
			// assumes the container bldg is the last part in the given address
			if (!isBldg(addr)) throw new ArgumentException("Given address isn't a location: " + addr);
			string[] parts = addr.Split (DELIM_CHAR_ARRAY);
			parts[parts.Length - 1] = "b(" + newX + "," + newY + ")";
			return string.Join (DELIM, parts);
		}

		public static string getContainingBldgAddress(string flr_addr) {
			string[] parts = flr_addr.Split (DELIM_CHAR_ARRAY);
			if (parts.Length == 1)
				return flr_addr;
			parts = parts.Take(parts.Length - 1).ToArray();
			// if it's a flr, get out to the containing bldg
			string last_part = parts [parts.Length - 1];
			if (last_part.StartsWith ("l")) {
				parts = parts.Take (parts.Length - 1).ToArray ();
			}
			return string.Join (DELIM, parts);
		}

		public static string replaceFlrLevel(string bldg_addr, int flr_level) {
			string[] parts = bldg_addr.Split (DELIM_CHAR_ARRAY);
			if (parts.Length <= 2) {
				// ground level, no coordinates
				return bldg_addr;
			}
			string bldg = null;
			string last_part = parts [parts.Length - 1];
			if (last_part.StartsWith ("b")) {
				bldg = last_part;
				parts = parts.Take (parts.Length - 1).ToArray ();
			}
			parts = parts.Take (parts.Length - 1).ToArray ();
			string part = "l" + flr_level;
			parts = Extensions.AppendToArray(parts, part);
			if (bldg != null) {
				parts = Extensions.AppendToArray(parts, bldg);
			}
			return string.Join (DELIM, parts);
		}



	}

	public static class Extensions {
		// helpers

		public static T[] AppendToArray<T> (this T[] original, T itemToAdd) {
			T[] finalArray = new T[ original.Length + 1 ];
			for(int i = 0; i < original.Length; i ++ ) {
				finalArray[i] = original[i];
			}
			finalArray[finalArray.Length - 1] = itemToAdd;
			return finalArray;
		}
	}
}
