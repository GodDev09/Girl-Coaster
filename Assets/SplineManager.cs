using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class SplineManager : MonoBehaviour
{
	public List<SplineComputer> splines;
	public List<TriggerGroup> individualTriggerGroups;

	private List<int> _babySplineEdges = new List<int>();
	private SplinePoint[] _splinePoints;
	private int _totalSplinePoints;
	

	public void Awake()
	{
		//splines[0].RebuildImmediate();
		//splines[0].rebuildOnAwake = true;
		GameObject.FindGameObjectWithTag("Player").GetComponent<SplineFollower>().spline = splines[0];
	}

	[ContextMenu("triggerGroups")]
	public void Trigg()
	{
		print(splines[0].triggerGroups[0].triggers[0].position);
		print(splines[0].GetPointPercent(3));
		
	}
	
	[ContextMenu("Join Splines")]
	public void JoinSplines()
	{
		_totalSplinePoints = 0;
		_babySplineEdges.Clear();
		individualTriggerGroups.Clear();
		
		foreach (var spline in splines)
		{
			_totalSplinePoints += spline.pointCount;
		}
		
		_splinePoints = new SplinePoint[_totalSplinePoints];

		GetAllTheSplinePoints();

		//splines.RemoveRange(1, splines.Count - 1);
		print("Joined splines successfully.");
	}
	
	private void GetAllTheSplinePoints()
	{
		//initialise empty array with spline1.length + splineN.len-1 ... n
		
		splines[0].GetPoints().CopyTo(_splinePoints,0);
		
		individualTriggerGroups.Add(splines[0].triggerGroups[0]);
		_babySplineEdges.Add(0);
		var copyToIndex = splines[0].pointCount - 1;
		_babySplineEdges.Add(copyToIndex);
		
		for (var i = 1; i < splines.Count; i++)
		{
			splines[i].GetPoints().CopyTo(_splinePoints, copyToIndex);
			copyToIndex += splines[i].pointCount - 1;
			
			individualTriggerGroups.Add(splines[i].triggerGroups[0]);
			_babySplineEdges.Add(copyToIndex);
		}

		splines[0].SetPoints(_splinePoints);
		ReArrangeTriggers();
		//splines[0].RebuildImmediate(true, true);
		//individualTriggerGroups.Clear();
	}

	private void ReArrangeTriggers()
	{
		for (var i = 0; i < splines.Count; i++)
		{
			var currentTriggerGroupsLength = individualTriggerGroups[i].triggers.Length;
						
			for (var j = 0; j < currentTriggerGroupsLength; j++)
			{
				var triggerPos = individualTriggerGroups[i].triggers[j].position;
				var point1 = splines[i].GetPointPercent(_babySplineEdges[i]);
				var point2 = splines[i].GetPointPercent(_babySplineEdges[i + 1]);

				var newTriggerPos = MyHelpers.LerpClampedDouble(point1, point2, triggerPos);
				splines[0].triggerGroups[0].triggers[j].position = newTriggerPos;
			}
		}
	}
}
