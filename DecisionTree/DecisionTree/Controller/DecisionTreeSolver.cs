namespace DecisionTree.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ViewModel;

    using Attribute = ViewModel.Attribute;

    /// <summary>
    /// Построитель дерева решений.
    /// </summary>
    public class DecisionTreeSolver
    {
        private readonly List<Attribute> _samples;

        private readonly DTreeSolveMethods _solveMethods;

        public DecisionTreeSolver(List<Attribute> samples)
        {
            _samples = samples;
            _solveMethods = new DTreeSolveMethods();
        }

        /// <summary>
        /// Построить дерево.
        /// </summary>
        /// <returns>Дерево принятия решений.</returns>
        public Node MountTree()
        {
            if (_samples.All(x => x.IsGoal))
            {
                var attr = _samples.Single(x => x.IsGoal);
                var mostCommon = _solveMethods.GetMostCommonValue(_samples);
                return new Node(attr, mostCommon);
            }

            if (_samples.Single(x => x.IsGoal).Values.Distinct().Count() == 1)
            {
                var attr = _samples.Single(x => x.IsGoal);
                return new Node(attr, attr.Values.First());
            }

            var maxGainAttr = _samples.Where(x => !x.IsGoal)
                .ToDictionary(attr => attr, attr => _solveMethods.Gain(_samples, attr))
                .OrderByDescending(x => x.Value)
                .First()
                .Key;

            var root = new Node(maxGainAttr, "");

            var goalValuesCount = _solveMethods.GoalValuesCount(_samples);
            _solveMethods.EntropyForSet = _solveMethods.CalcEntropy(goalValuesCount);

            if (maxGainAttr.IsDiscrete)
            {
                foreach (var distinctVal in maxGainAttr.Values.Distinct())
                {
                    var curSamples = _samples
                        .Where(x => x.Name != maxGainAttr.Name)
                        .Select(x => x.FilterValues(
                            maxGainAttr.Values
                            .Select((val, index) => new {index, val})
                            .Where(val => val.val == distinctVal)
                            .Select(val => val.index)
                            .ToArray()))
                        .ToList();
                    var curNode = new Node(maxGainAttr, distinctVal) {Operation = Operation.Eq};
                    root.Children.Add(curNode);

                    var eqDt = new DecisionTreeSolver(curSamples);
                    curNode.Children.Add(eqDt.MountTree());
                }
            }
            else
            {
                var resultAttr = _samples.Single(x => x.IsGoal);
                var maxEntropyValue = _solveMethods.CalcMaxEntropy(_samples, maxGainAttr).Item1;
                var lessSamples = _samples
                        .Where(x => x.Name != maxGainAttr.Name)
                        .Select(x => x.FilterValues(
                            maxGainAttr.Values
                            .Select((val, index) => new { index, val })
                            .Where(val => Convert.ToDouble(val.val) < maxEntropyValue)
                            .Select(val => val.index)
                            .ToArray()))
                        .ToList();
                var dtLess = new DecisionTreeSolver(lessSamples);

                var curNode = new Node(maxGainAttr, maxEntropyValue.ToString()) {Operation = Operation.Less};
                root.Children.Add(curNode);
                curNode.Children.Add(
                    lessSamples.All(x => x.Values.Count == 0)
                        ? new Node(resultAttr, _solveMethods.GetMostCommonValue(_samples)) { Operation = Operation.Less}
                        : dtLess.MountTree());

                var geSamples = _samples
                        .Where(x => x.Name != maxGainAttr.Name)
                        .Select(x => x.FilterValues(
                            maxGainAttr.Values
                            .Select((val, index) => new { index, val })
                            .Where(val => Convert.ToDouble(val.val) >= maxEntropyValue)
                            .Select(val => val.index)
                            .ToArray()))
                        .ToList();
                var dtGe = new DecisionTreeSolver(geSamples);
                var curNode2 = new Node(maxGainAttr, maxEntropyValue.ToString()) {Operation = Operation.MoreEq};
                root.Children.Add(curNode2);
                curNode2.Children.Add(
                    geSamples.All(x => x.Values.Count == 0)
                        ? new Node(resultAttr, _solveMethods.GetMostCommonValue(_samples)) { Operation = Operation.MoreEq}
                        : dtGe.MountTree());
            }

            return root;
        }
    }
}
