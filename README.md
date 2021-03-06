![IFSMetrics](IFSMetrics-logo-ext.png)

## Introduction
*IFSMetrics* is a package containing modules whereby you can build intuitionistic fuzzy sets (IFSs) from real-world data, perform similarity comparisons between those IFSs and generate a comprehensive report with the results of such comparisons. As such, *IFSMetrics* aims for the design of similarity measures that perform reliable comparisons between IFSs.

The current version of *IFSMetrics* consists of three modules: *IFSBuilder*, *IFSComparer* and *IFSSimReporter*. Since these modules are written in C#, you can develop them on platforms like Linux, Windows and macOS where the .Net framework is supported.

## Requirements

### External components
IFSMetrics uses the following external components:

| IFSMetrics Module                   | External Component(s)      
| :---------------------------------- | :-------------
| *IFSBuilder* (Building process)     | [svmlight](http://svmlight.joachims.org)
| *IFSSimReporter* (Reporting process)| [gnuplot](http://www.gnuplot.info), [pdflatex](https://www.tug.org/applications/pdftex/)

### Dataset
In the current version of IFSMetrics, IFSs are derived from the RCV1-v2 dataset:  

- Lewis, D. D.  RCV1-v2/LYRL2004: The LYRL2004 Distribution of the RCV1-v2 Text Categorization Test Collection (5-Mar-2015 Version). http://www.jmlr.org/papers/volume5/lewis04a/lyrl2004_rcv1v2_README.htm.

## Getting started with IFSMetrics
The [getting started](https://github.com/ifsmetrics-lab/ifsmetrics/wiki/Getting-Started) wiki page explains how to start using IFSMetrics.

## License
IFSMetrics is released under the [Apache License, Version 2.0](LICENSE).

## Further reading
- IFSBuilder Tutorials
  - [How to configure the scenarios, categories and parameters that will be used during a building process](https://github.com/ifsmetrics-lab/ifsmetrics/wiki/How-To-Configure-A-Building-Process)
  - [How to perform a building process with MonoDevelop](https://github.com/ifsmetrics-lab/ifsmetrics/wiki/How-To-Perform-A-Building-Process-With-MonoDevelop)
- IFSComparer Tutorials
  - [How to configure the scenarios, categories and similarity measures that will be used during a comparison  process](https://github.com/ifsmetrics-lab/ifsmetrics/wiki/How-To-Configure-A-Comparison-Process)
  - [How to perform a comparison process with MonoDevelop](https://github.com/ifsmetrics-lab/ifsmetrics/wiki/How-To-Perform-A-Comparison-Process-With-MonoDevelop)
  - [How to develop a new similarity measure](https://github.com/ifsmetrics-lab/ifsmetrics/wiki/How-To-Develop-A-New-Similarity-Measure)
- IFSSimReporter Tutorials
  - [How to configure the reporting preferences that will be used during a reporting process](https://github.com/ifsmetrics-lab/ifsmetrics/wiki/How-To-Configure-A-Reporting-Process)
  - [How to perform a reporting process with MonoDevelop](https://github.com/ifsmetrics-lab/ifsmetrics/wiki/How-To-Perform-A-Reporting-Process-With-MonoDevelop)
