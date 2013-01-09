using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class Sphero {
	
// How do we manage multiple robots from here?	
#if UNITY_ANDROID	
	AndroidJavaObject m_JavaSphero;
#endif
	
	// Bluetooth Info Inner Data Structure Class
	private class BluetoothInfo {
		private string m_Name;
	    public string Name
	    {
			get{ return this.m_Name; }
			set{ this.m_Name = value; }
	    }
		private string m_Address;
		public string Address
	    {
			get{ return this.m_Address; }
			set{ this.m_Address = value; }
	    }
		
		public BluetoothInfo(string name, string address) {
			m_Name = name;
			m_Address = address;
		}
	}
	private BluetoothInfo m_BluetoothInfo; 
	
	// Current Sphero Color
	private int m_Color;
	public int Color 
	{
		get{ return this.m_Color; }
		set{ this.m_Color = value; }
	}
	
	// Connection State
	public enum Connection_State { Failed, Disconnected, Connecting, Connected };
	private Connection_State m_ConnectionState = Connection_State.Disconnected;
	public Connection_State ConnectionState
	{
		get{ return this.m_ConnectionState; }
		set{ this.m_ConnectionState = value; }
	}
	
#if UNITY_ANDROID	
	/*
	 * Default constructor used for Android 
	 */ 
	public Sphero(AndroidJavaObject sphero) {		
		m_JavaSphero = sphero;
	}
	
	/*
	 * More detailed constructor used for Android 
	 */ 
	public Sphero(AndroidJavaObject sphero, string bt_name, string bt_address) {		
		m_JavaSphero = sphero;
		m_BluetoothInfo = new BluetoothInfo(bt_name, bt_address);
	}
#endif
	
	/*
	 * Default constructor used for iOS 
	 */ 
	public Sphero() {}
	
	/*
	 * Change Sphero's color to desired output
	 * @param red the amount of red from (0.0 - 1.0) intensity
	 * @param green the amount of green from (0.0 - 1.0) intensity
	 * @param blue the amount of blue from (0.0 - 1.0) intensity
	 */
	public void SetRGBLED(float red, float green, float blue) {
		#if UNITY_ANDROID	
			using( AndroidJavaClass jc = new AndroidJavaClass("orbotix.robot.base.RGBLEDOutputCommand") ) {
				jc.CallStatic("sendCommand",m_JavaSphero,red*255,green*255,blue*255);
			}
		#elif UNITY_IPHONE
			SpheroBridge.SetRGB(red,green,blue);
		#endif
		
		// Set the alpha to 1
		m_Color = 255;
		m_Color = m_Color << 8;
		// Set red bit and shift 8 left
		m_Color += (int)(255 * red);
		m_Color = m_Color << 8;
		// Set green bit and shift 8 left
		m_Color += (int)(255 * green);
		m_Color = m_Color << 8;
		// Set blue bit
		m_Color += (int)(255 * blue);
	}
	
		/*
	 * Change Sphero's color to desired output
	 * @param color is a hexadecimal representation of color
	 */
	public void SetRGBLED(int color) {
		
		int red = (color & 0x00FF0000) >> 16;
		int green = (color & 0x0000FF00) >> 8;
		int blue = color & 0x000000FF;
		
		#if UNITY_ANDROID	
			using( AndroidJavaClass jc = new AndroidJavaClass("orbotix.robot.base.RGBLEDOutputCommand") ) {
				jc.CallStatic("sendCommand",m_JavaSphero,red,green,blue);
			}
		#elif UNITY_IPHONE
			SpheroBridge.SetRGB(red/255,green/255,blue/255);
		#endif
		
		m_Color = color;
	}
	
	/*
	 * Get the current red color of the ball 
	 */ 
	public float GetRedIntensity() {
		return (m_Color & 0x00FF0000) >> 16;
	}
	
	/*
	 * Get the current green color of the ball 
	 */ 
	public float GetGreenIntensity() {
		return (m_Color & 0x0000FF00) >> 8;
	}
	
	/*
	 * Get the current blue color of the ball 
	 */ 
	public float GetBlueIntensity() {
		return m_Color & 0x000000FF;
	}
	
	/*
	 * Get the Name of the Sphero. (i.e. Sphero-PRR)
	 */
	public string GetName() {
		return m_BluetoothInfo.Name;
	}
}