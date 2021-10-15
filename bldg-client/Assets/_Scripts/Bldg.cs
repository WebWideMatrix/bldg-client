using System;
using System.Collections.Generic;

namespace Models
{
	[Serializable]
	public class Bldg
	{
		public int x;

		public int y;

		public string web_url;

		public string summary;

		public bool is_composite;

		public string flr;

		public string entity_type;

		public string name;

		public string state;

		public string category;

		public string[] tags;

		public string address;

		public string picture_url;

		public override string ToString(){
			return UnityEngine.JsonUtility.ToJson (this, true);
		}
	}

	[Serializable]
	public struct ResidentAttributes
	{

	}


	[Serializable]
	public struct Resident
	{
		public string alias;
		public int direction;
		public string email;
		public string home_bldg;
		public int id;
		public string text;
		public bool is_online;
		public DateTime last_login_at;
		public string location;
		public string name;
		public ResidentAttributes other_attributes;
		public string[] previous_messages;
		public string session_id;
		public int x;
		public int y;
		public string flr;
	}


	[Serializable]
	public struct LoginRequest
	{
		public string email;
	}

	[Serializable]
	public struct LoginResponse
	{
		public Resident data;
	}
	
	[Serializable]
	public struct MoveAction
	{
		public string resident_email;
		public string action_type;
		public string move_location;
		public int move_x;
		public int move_y;
	}

	[Serializable]
	public struct SayAction
	{
		public string resident_email;
		public string action_type;
		public string say_speaker;
		public string say_text;
		public long say_time;
		public string say_flr;
		public string say_location;
		public string say_mimetype;
		public string say_recipient;
	}

}

