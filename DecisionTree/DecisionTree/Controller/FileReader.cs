﻿namespace DecisionTree.Controller
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using ViewModel;

    using Attribute = ViewModel.Attribute;

    public class FileReader
    {
        private readonly string _fileName;

        public string GoalAttrName { get; set; }

        public FileReader(string fileName)
        {
            _fileName = fileName;
        }

        public List<Attribute> ReadAttributes()
        {
            var attrs = ReadHeaders();
            FillAttrsWithData(attrs);
            return attrs;
        }

        private List<Attribute> ReadHeaders()
        {
            var headers = File.ReadAllLines(_fileName).First().Split('\t').ToList();
            
            var attrs = headers.Select(x => new Attribute(x.Split('(', ')').First(), x.Split('(', ')').Skip(1).First())).ToList();

            GoalAttrName = attrs.Single(x => x.IsGoal).Name;

            return attrs;
        }

        private void FillAttrsWithData(List<Attribute> attrs)
        {
            var inputTextLines = File.ReadAllLines(_fileName);
            for (var row = 1; row < inputTextLines.Length; row++)
            {
                var strValues = inputTextLines[row].Split('\t');
                for (var valNum = 0; valNum < strValues.Length; valNum++)
                {
                    attrs[valNum].Values.Add(strValues[valNum]);
                    if (attrs[valNum].IsDiscrete && !attrs[valNum].PossibleValues.Contains(strValues[valNum]))
                    {
                        attrs[valNum].PossibleValues.Add(strValues[valNum]);
                    }
                }
            }
        }
    }
}
