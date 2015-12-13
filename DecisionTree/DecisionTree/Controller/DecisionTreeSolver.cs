namespace DecisionTree.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ViewModel;

    using Attribute = ViewModel.Attribute;

    public class DecisionTreeSolver
    {
        private readonly List<Attribute> _samples;

        private double _entropyForSet { get; set; }

        public DecisionTreeSolver(List<Attribute> samples)
        {
            _samples = samples;
        }

        public Node MountTree()
        {
            if (_samples.All(x => x.IsGoal))
            {
                var attr = _samples.Single(x => x.IsGoal);
                var mostCommon = GetMostCommonValue(_samples);
                return new Node(attr, mostCommon);
            }

            if (_samples.Single(x => x.IsGoal).Values.Distinct().Count() == 1)
            {
                var attr = _samples.Single(x => x.IsGoal);
                return new Node(attr, attr.Values.First());
            }

            var maxGainAttr = _samples.Where(x => !x.IsGoal)
                .ToDictionary(attr => attr, attr => Gain(_samples, attr))
                .OrderByDescending(x => x.Value)
                .First()
                .Key;

            var root = new Node(maxGainAttr, "");

            var goalValuesCount = GoalValuesCount(_samples);
            _entropyForSet = CalcEntropy(goalValuesCount);

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
                var maxEntropyValue = CalcMaxEntropy(_samples, maxGainAttr).Item1;
                var lessSamples = _samples
                        .Where(x => x.Name != maxGainAttr.Name)
                        .Select(x => x.FilterValues(
                            maxGainAttr.Values
                            .Select((val, index) => new { index, val })
                            .Where(val => Convert.ToInt32(val.val) < maxEntropyValue)
                            .Select(val => val.index)
                            .ToArray()))
                        .ToList();
                var dtLess = new DecisionTreeSolver(lessSamples);

                var curNode = new Node(maxGainAttr, maxEntropyValue.ToString()) {Operation = Operation.Less};
                root.Children.Add(curNode);
                curNode.Children.Add(
                    lessSamples.All(x => x.Values.Count == 0)
                        ? new Node(resultAttr, GetMostCommonValue(_samples))
                        : dtLess.MountTree());

                var geSamples = _samples
                        .Where(x => x.Name != maxGainAttr.Name)
                        .Select(x => x.FilterValues(
                            maxGainAttr.Values
                            .Select((val, index) => new { index, val })
                            .Where(val => Convert.ToInt32(val.val) >= maxEntropyValue)
                            .Select(val => val.index)
                            .ToArray()))
                        .ToList();
                var dtGe = new DecisionTreeSolver(geSamples);
                var curNode2 = new Node(maxGainAttr, maxEntropyValue.ToString()) {Operation = Operation.MoreEq};
                root.Children.Add(curNode2);
                curNode2.Children.Add(
                    geSamples.All(x => x.Values.Count == 0)
                        ? new Node(resultAttr, GetMostCommonValue(_samples))
                        : dtGe.MountTree());
            }

            return root;
        }

        private Tuple<int, double> CalcMaxEntropy(List<Attribute> sample, Attribute curAttr)
        {
            var totalDic = new Dictionary<int, double>();
            foreach (var t in curAttr.Values)
            {
                var col = sample.Single(x => x.Name == curAttr.Name).Values.ToList();
                var totalCountValues = col.Count;
                var curT = int.Parse(t);
                var lessT = new List<int>();
                for (var j = 0; j < totalCountValues; j++)
                {
                    var curX = int.Parse(col[j]);
                    if (curX < curT)
                    {
                        lessT.Add(j);
                    }
                }
                var pLess = (double)lessT.Count / totalCountValues;
                var dictValsLess = CalcDiffsOnRes(sample, lessT);
                var lessEntropy = CalcEntropy(dictValsLess);

                var geT = new List<int>();
                for (var j = 0; j < totalCountValues; j++)
                {
                    var curX = int.Parse(col[j]);
                    if (curX >= curT)
                    {
                        geT.Add(j);
                    }
                }
                var pGe = (double)geT.Count / totalCountValues;
                var dictValsGe = CalcDiffsOnRes(sample, geT);
                var geEntropy = CalcEntropy(dictValsGe);

                var entropyForT = pLess * lessEntropy + pGe * geEntropy;
                var resEntropy = _entropyForSet - entropyForT;

                if (!totalDic.ContainsKey(curT))
                {
                    totalDic.Add(curT, resEntropy);
                }
            }

            var maxEntropy = totalDic.Max(x => x.Value);
            var best = totalDic.First(x => x.Value.Equals(maxEntropy));
            return Tuple.Create(best.Key, best.Value);
        }

        private Dictionary<string, int> CalcDiffsOnRes(List<Attribute> sample, List<int> indexes) => 
            sample
                .Single(x => x.IsGoal)
                .Values
                    .Where((x, i) => indexes.Contains(i))
                    .Distinct()
                    .ToDictionary(x => x, x => sample
                        .Single(j => j.IsGoal)
                        .Values
                            .Where((x2, i) => indexes.Contains(i))
                            .Count(y => x == y));

        private double CalcEntropy(Dictionary<string, int> goalValsCount) => goalValsCount
            .Select(dictVal => (double)dictVal.Value / goalValsCount.Sum(x => x.Value))
            .Select(curPart => curPart > 0 ? -curPart * Math.Log(curPart, 2) : 0).Sum();

        private double Gain(List<Attribute> samples, Attribute attribute)
        {
            var dict = attribute
                .Values
                .Distinct()
                .ToDictionary(x => x, x => attribute.Values.Count(y => y == x));

            double sum = 0;

            if (!attribute.IsDiscrete)
            {
                var maxEntropyValue = CalcMaxEntropy(_samples, attribute);
                return maxEntropyValue.Item2;
            }
            foreach (var val in dict)
            {
                var valsDict = GetValuesToAttribute(samples, attribute, val.Key);
                var entropy = CalcEntropy(valsDict);
                var valProb = (double)val.Value / dict.Sum(x => x.Value);

                var targEntropy = valProb * entropy;
                sum += targEntropy > 0 ? targEntropy : 0;
            }

            var result = _entropyForSet - sum;
            attribute.InfoGain = result;
            return result;
        }

        private Dictionary<string, int> GetValuesToAttribute(List<Attribute> samples, Attribute attribute, string value)
        {
            var dictVals = new Dictionary<string, int>();

            for (var i = 0; i < attribute.Values.Count; i++)
            {
                if (attribute.Values[i] == value)
                {
                    var resValue = samples.Single(x => x.IsGoal).Values[i];
                    if (dictVals.ContainsKey(resValue))
                    {
                        dictVals[resValue]++;
                    }
                    else
                    {
                        dictVals.Add(resValue, 1);
                    }
                }
            }
            return dictVals;
        }

        private Dictionary<string, int> GoalValuesCount(List<Attribute> samples)
        {
            return samples.Single(x => x.IsGoal)
                .Values.GroupBy(x => x)
                .ToDictionary(x => x.Key, y => y.Count());
        }

        private string GetMostCommonValue(List<Attribute> samples)
        {
            var count = GoalValuesCount(samples);
            return count.First(x => x.Value == count.Max(y => y.Value)).Key;
        }
    }
}
