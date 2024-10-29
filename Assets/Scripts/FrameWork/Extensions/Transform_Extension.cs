
using UnityEngine;

public static class TransformExtension
{
	// change parent and reset (local) matrix
	public static void ChangeParent(this Transform _this, Transform parent, bool resetMat = true)
	{
		if (_this == null)
			return;

		_this.SetParent(parent);
		if (resetMat)
		{
			_this.localScale = Vector3.one;
			_this.localEulerAngles = Vector3.zero;
			_this.localPosition = Vector3.zero;
		}
	}

	public static void Reset(this Transform _this)
	{
		if (_this == null) {
			return;
		}
		_this.localScale = Vector3.one;
		_this.localRotation = Quaternion.identity;
		_this.localPosition = Vector3.zero;
	}

	/// <summary>
	/// active的子物体个数 (最大索引255)
	/// </summary>
	/// <param name="_this"></param>
	/// <returns></returns>
	public static int GetActiveChildCount(this Transform _this)
	{
		byte count=0;
		for (int i = 0,c = _this.childCount % 255; i < c ; i++)
		{
			if (_this.GetChild(i).gameObject.activeSelf)
			{
				count++;
			}
		}
		return count;
	}
}

class PositionFollower: MonoBehaviour
{
	public Transform reference = null;
	
	void Update()
	{
		if (reference)
			transform.position = reference.position;
	}
}

class DistanceWatcher: MonoBehaviour
{
	public Transform reference = null;
	void Update()
	{
		if (!reference || Vector3.SqrMagnitude(reference.position - transform.position) < 2)
			gameObject.SetActive(false);
	}
}
