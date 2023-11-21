// Simple helper class that allows you to serialize System.Type objects.
// Use it however you like, but crediting or even just contacting the author would be appreciated (Always 
// nice to see people using your stuff!)
//
// Written by Bryan Keiren (http://www.bryankeiren.com)

using UnityEngine;
using System.Runtime.Serialization;

[System.Serializable]
public class SerializableSystemType
{
	[SerializeField]
	private string name;
	
	[SerializeField]
	private string assemblyQualifiedName;
	
	[SerializeField]
	private string assemblyName;
	
	private System.Type _systemType;
	
	
	public string Name
	{
		get { return name; }
	}

	public string AssemblyQualifiedName
	{
		get { return assemblyQualifiedName; }
	}
	
	public string AssemblyName
	{
		get { return assemblyName; }
	}
	
	public System.Type SystemType
	{
		get 	
		{
			if (_systemType == null)	
			{
				GetSystemType();
			}
			return _systemType;
		}
	}
	
	private void GetSystemType()
	{
		_systemType = System.Type.GetType(assemblyQualifiedName);
	}

	public SerializableSystemType(System.Type systemType) 
	{
		this._systemType = systemType;
		name = systemType.Name;
		assemblyQualifiedName = systemType.AssemblyQualifiedName;
		assemblyName = systemType.Assembly.FullName;
	}
	
	public override bool Equals( System.Object obj )
	{
		SerializableSystemType temp = obj as SerializableSystemType;
		if ((object)temp == null)
		{
			return false;
		}
		return this.Equals(temp);
	}
	
	public bool Equals( SerializableSystemType obj )
	{
		return obj.SystemType.Equals(SystemType);
	}
	
	public static bool operator ==( SerializableSystemType a, SerializableSystemType b )
	{
	    // If both are null, or both are same instance, return true.
	    if (System.Object.ReferenceEquals(a, b))
	    {
	        return true;
	    }
	
	    // If one is null, but not both, return false.
	    if (((object)a == null) || ((object)b == null))
	    {
	        return false;
	    }
	
	    return a.Equals(b);
	}
	
	public static bool operator !=( SerializableSystemType a, SerializableSystemType b )
	{
	    return !(a == b);
	}
}
