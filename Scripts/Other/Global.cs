using System.Collections;
using System.Collections.Generic;


public static class Global {
	public static string GenerateID<T> () {
		return GenerateID (typeof(T));
	}

	public static string GenerateID (System.Type type) {
		return type.Name;
	}

	public static string PrepareNotification<T> () {
		return PrepareNotification (typeof(T));
	}

	public static string PrepareNotification (System.Type type) {
		return string.Format ("{0}.PrepareNotification", type.Name);
	}

	public static string PerformNotification<T> () {
		return PerformNotification (typeof(T));
	}

	public static string PerformNotification (System.Type type) {
		return string.Format ("{0}.PerformNotification", type.Name);
	}

	public static string ValidateNotification<T> () {
		return ValidateNotification (typeof(T));
	}

	public static string ValidateNotification (System.Type type) {
		return string.Format ("{0}.ValidateNotification", type.Name);
	}

	public static string CancelNotification<T> () {
		return CancelNotification (typeof(T));
	}

	public static string CancelNotification (System.Type type) {
		return string.Format ("{0}.CancelNotification", type.Name);
	}
}