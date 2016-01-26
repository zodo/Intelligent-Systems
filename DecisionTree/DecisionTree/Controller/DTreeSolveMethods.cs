namespace DecisionTree.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Attribute = ViewModel.Attribute;

    public class DTreeSolveMethods
    {
        public double EntropyForSet { get; set; }

        public Tuple<double, double> CalcMaxEntropy(List<Attribute> sample, Attribute curAttr)
        {
            var totalDic = new Dictionary<double, double>();
            foreach (var t in curAttr.Values)
            {
                var col = sample.Single(x => x.Name == curAttr.Name).Values.ToList();
                var totalCountValues = col.Count;
                var curT = double.Parse(t);
                var lessT = new List<int>();
                for (var j = 0; j < totalCountValues; j++)
                {
                    var curX = double.Parse(col[j]);
                    if (curX < curT)
                    {
                        lessT.Add(j);
                    }
                }
                var pLess = (double)lessT.Count / totalCountValues;
                var dictValsLess = CalcDiffsOnRes(sample, lessT);
                var lessEntropy = GetEntropy(dictValsLess);

                var geT = new List<int>();
                for (var j = 0; j < totalCountValues; j++)
                {
                    var curX = double.Parse(col[j]);
                    if (curX >= curT)
                    {
                        geT.Add(j);
                    }
                }
                var pGe = (double)geT.Count / totalCountValues;
                var dictValsGe = CalcDiffsOnRes(sample, geT);
                var geEntropy = GetEntropy(dictValsGe);

                var entropyForT = pLess * lessEntropy + pGe * geEntropy;
                var resEntropy = EntropyForSet - entropyForT;

                if (!totalDic.ContainsKey(curT))
                {
                    totalDic.Add(curT, resEntropy);
                }
            }

            var maxEntropy = totalDic.Max(x => x.Value);
            var best = totalDic.First(x => x.Value.Equals(maxEntropy));
            return Tuple.Create(best.Key, best.Value);
        }

        public Dictionary<string, int> CalcDiffsOnRes(List<Attribute> sample, List<int> indexes) =>
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

        public double GetEntropy(Dictionary<string, int> goalValsCount) => goalValsCount
            .Select(dictVal => (double)dictVal.Value / goalValsCount.Sum(x => x.Value))
            .Select(curPart => curPart > 0 ? -curPart * Math.Log(curPart, 2) : 0).Sum();

        public double Gain(List<Attribute> samples, Attribute attribute)
        {
            var dict = attribute
                .Values.GroupBy(x => x)
                .ToDictionary(x => x.Key, y => y.Count());

            double sum = 0;

            if (!attribute.IsDiscrete)
            {
                var maxEntropyValue = CalcMaxEntropy(samples, attribute);
                return maxEntropyValue.Item2;
            }
            foreach (var val in dict)
            {
                var valsDict = GetValuesToAttribute(samples, attribute, val.Key);
                var entropy = GetEntropy(valsDict);
                var valProb = (double)val.Value / dict.Sum(x => x.Value);

                var targEntropy = valProb * entropy;
                sum += targEntropy > 0 ? targEntropy : 0;
            }

            var result = EntropyForSet - sum;
            attribute.InfoGain = result;
            return result;
        }

        public Dictionary<string, int> GetValuesToAttribute(List<Attribute> samples, Attribute attribute, string value)
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

        public Dictionary<string, int> GoalValuesCount(List<Attribute> samples)
        {
            return samples.Single(x => x.IsGoal)
                .Values.GroupBy(x => x)
                .ToDictionary(x => x.Key, y => y.Count());
        }

        public string GetMostFrequentlyValue(List<Attribute> samples)
        {
            var count = GoalValuesCount(samples);
            return count.First(x => x.Value == count.Max(y => y.Value)).Key;
        }
    }
}
