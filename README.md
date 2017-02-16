# IFSMetrics

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

### Building IFSMetrics
To build the IFSMetrics solution, you can follow these steps:

1. Clone the IFSMetrics repository:
```
$ git clone https://github.com/ifsmetrics-lab/ifsmetrics.git
```
2. Open the solution file
[IFSMetrics.sln](IFSMetrics.sln) with
[Visual Studio](http://www.visualstudio.com) or
[MonoDevelop](http://www.MonoDevelop.com).
3. Use the *Build Solution* (Visual Studio) or *Build All* (MonoDevelop)
command to build the complete solution.

### Running an experiment
During an experiment, several similarity measures can be tested to determine their suitability for comparisons between intuitionistic fuzzy sets (IFSs) that represent experience-based evaluations. An experiment goes through three stages: a building process, a comparison process and a reporting process.
During the building process, IFSs will be derived from the RCV1-v2 dataset after evaluating to which degree newswire stories belong to one or more categories according to several learning scenarios. After that those IFSs will be compared to each other during the comparison process. Finally, the results of those comparisons are summarized during the reporting process.

#### Building process
To derive IFSs from the RCV1-v2 dataset, you must configure the parameters that will be used during a building process. After that, you can perform the process through *IFSBuilder*. To do so, you can enter the command

```bash
$ ./IFSBuilder.exe experiment-configuration.xml
```
where  `experiment-configuration.xml` is a file containing the configuration of the process.

The *experiment-configuration* file has the following structure:

```xml
<?xml version="1.0" encoding="utf-8"?>
<ExperimentConfig xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <BuildingProcessParams>
    <LearningParams/>
    <EvaluationParams/>
    <Scenarios/>
    <Categories/>
  </BuildingProcessParams>
  <ComparisonProcessParams/>
  <ReportingProcessParams/>
  <WorkingDir/>
</ExperimentConfig>
```
Notice that this file contains the configuration of the building, comparison and reporting processes. Notice also that the parameters of a building process are `LearningParams`, `EvaluationParams`, `Scenarios`, `Categories` and `WorkingDir`.

In `LearningParams`, you can specify the path of the file containing the newswire stories (`DocumentsFile`), the path of the file containing the assigned categories (`DocumentCategoryRelationsFile`), and the number of stories (`nDocuments`) that will be used during the learning process.
Also, you can specify the path of `svm_learn`, which is part of *SVMLight*, as well as the directories where it will store the *SVMModels* (`SVMModelsDir`) and the *SVMDocumentVectors* (`SVMDocumentVectorsDir`).

```xml
...
<LearningParams>
  <nDocuments>13149</nDocuments>
  <DocumentsFile>/home/your-user/datasets/RCV1/lyrl2004_tokens_train.dat</DocumentsFile>
  <DocumentCategoryRelationsFile>/home/your-user/datasets/RCV1/rcv1-v2.topics.qrels</DocumentCategoryRelationsFile>
  <SVMLearningExecFile>/home/your-user/tools/SVMLight/svm_learn</SVMLearningExecFile>
  <SVMModelsDir>/home/your-user/output/IFSMetrics/SVMModels/</SVMModelsDir>
  <SVMDocumentVectorsDir>/home/your-user/output/IFSMetrics/SVMVectors/</SVMDocumentVectorsDir>
</LearningParams>
...
```

  In `EvaluationParams`, you can specify the sources of the newswire stories that will be used during the evaluation process. Each source will be described by an identifier (`Code`), the path of the document containing newswire stories (`DocumentsFile`), and the number of stories (`nDocuments`) that will be evaluated from this file. You can also establish the number of stories that will contain each evaluation batch (`nDocumentsInBatch`), as well as the directories where the IFSs (`IFSsDir`) and the *SVMDocumentVectors* (`SVMDocumentVectorsDir`) will be stored.

  ```xml
...
<EvaluationParams>
	<Sources>
		<Source>
			<Code>1</Code>
			<DocumentsFile>/home/your-user/datasets/RCV1/lyrl2004_tokens_test_pt0.dat</DocumentsFile>
			<nDocuments>500</nDocuments>
		</Source>
		<Source>
			<Code>2</Code>
			<DocumentsFile>/home/your-user/datasets/RCV1/lyrl2004_tokens_test_pt1.dat</DocumentsFile>
			<nDocuments>500</nDocuments>
		</Source>
		<Source>
			<Code>3</Code>
			<DocumentsFile>/home/your-user/datasets/RCV1/lyrl2004_tokens_test_pt2.dat</DocumentsFile>
			<nDocuments>500</nDocuments>
		</Source>
		<Source>
			<Code>4</Code>
			<DocumentsFile>/home/your-user/datasets/RCV1/lyrl2004_tokens_test_pt3.dat</DocumentsFile>
			<nDocuments>500</nDocuments>
		</Source>
	</Sources>
	<IFSsDir>/home/your-user/output/IFSMetrics/IFSs/</IFSsDir>
	<nDocumentsInBatch>50</nDocumentsInBatch>
	<SVMDocumentVectorsDir>/home/your-user/output/IFSMetrics/SVMVectors/</SVMDocumentVectorsDir>
</EvaluationParams>
...
```

In `Scenarios`, you can define the learning scenarios that will be used during the building process. Each scenario will have an identifier (`Code`), a description (`Description`), as well as the percentage of opposite examples (`OppositesPercentage`) that will be included in the scenario.

```xml
<Scenarios>
	<Scenario>
		<Code>R0</Code>
		<Description>R0</Description>
		<OppositesPercentage>0</OppositesPercentage>
	</Scenario>
	<Scenario>
		<Code>R20</Code>
		<Description>R20</Description>
		<OppositesPercentage>0.2</OppositesPercentage>
	</Scenario>
	<Scenario>
		<Code>R40</Code>
		<Description>R40</Description>
		<OppositesPercentage>0.4</OppositesPercentage>
	</Scenario>
	<Scenario>
		<Code>R60</Code>
		<Description>R60</Description>
		<OppositesPercentage>0.6</OppositesPercentage>
	</Scenario>
	<Scenario>
		<Code>R80</Code>
		<Description>R80</Description>
		<OppositesPercentage>0.8</OppositesPercentage>
	</Scenario>
	<Scenario>
		<Code>R100</Code>
		<Description>R100</Description>
		<OppositesPercentage>1</OppositesPercentage>
	</Scenario>
</Scenarios>

```

In `Categories`, you can specify the categories to be used during both the learning and the comparison processes. Each category will have an identifier (`Code`) and a description (`Description`). The identifiers of the categories are the given in RCV1-v2.

```xml
<Categories>
	<Category>
		<Code>E11</Code>
		<Description>E11 Category</Description>
	</Category>
	<Category>
		<Code>ECAT</Code>
		<Description>ECAT Category</Description>
	</Category>
	<Category>
		<Code>GJOB</Code>
		<Description>GJOB Category</Description>
	</Category>
</Categories>

```
Finally, you can establish in `WorkingDir` a directory where the log of a building process and other temporal files will be stored.

#### Comparison process
To compare the IFSs resulting from the building process, you must configure the similarity measures, as well as the sources, scenarios and categories that will be used during the comparison process. After that, you can run the process through *IFSComparer*. To do so, you can enter the command

```bash
$ ./IFSComparer.exe experiment-configuration.xml
```
where  `experiment-configuration.xml` is a file containing the configuration of the process -- which can be the same file used during the building process.

The parameters used during the comparison process are structured as follows:

```xml
<?xml version="1.0" encoding="utf-8"?>
<ExperimentConfig xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <BuildingProcessParams/>
  <ComparisonProcessParams>
    <!--These parameters should have the same or a subset of the values used during the building process-->
    <Sources/>
    <Scenarios/>
    <Categories/>
    <IFSsDir/>
    <WorkingDir/>
    <!-- This parameter should have the value specified in `BuildingProcessParam/EvaluationParams/nDocumentsInBatch`  -->
    <nDocumentsInEvaluationBatch/>
    <!-- These are different parameters..  -->
    <SimilaritiesFile/>
    <ReferencesFile/>
    <ReferentScenario/>
    <Measures/>
  </ComparisonProcessParams>
  <ReportingProcessParams/>
  <WorkingDir/>
</ExperimentConfig>
```
The parameters `Sources`, `Scenarios`, `Categories`, `IFSsDir` and `WorkingDir` in `ComparisonProcessParams` should have the same (or a subset of the) values used during the building process. Similarly, `nDocumentsInEvaluationBatch` should have the value specified in `BuildingProcessParam/EvaluationParams/nDocumentsInBatch`. Thus, you can copy and paste those parameters from `BuildingProcessParams`.

While in `SimilaritiesFile` you can specify the path of the file that will contain the results of the comparisons, in `ReferencesFile` you can establish the path of the file that will have the references of the similarity measures used during the comparison process.

```xml
..
<SimilaritiesFile>/home/your-user/output/IFSMetrics/Similarities/similarities.csv</SimilaritiesFile>
<ReferencesFile>/home/your-user/output/IFSMetrics/Similarities/references.xml</ReferencesFile>
...
```

In `ReferentScenario`, you can indicate the scenario having the IFSs that will be used as point of references during the comparison process.

```xml
...
<ReferentScenario>
  <Code>R0</Code>
  <Description>R0</Description>
  <OppositesPercentage>0</OppositesPercentage>
</ReferentScenario>
...
```

In `Measures`, you can establish the (configuration of the) similarity measures that will be used. Each measure will have a configuration depending on its definition.

```xml
...
<Measures>
	<AgreementOnDecisionSM>
		<Code>AoD</Code>
	</AgreementOnDecisionSM>
	<VBSM>
		<Code>VB-0.5</Code>
		<Alpha>0.5</Alpha>
	</VBSM>
	<HammingSM>
		<Code>H3D</Code>
		<HammingType>ThreeDimensions</HammingType>
	</HammingSM>
	<EuclideanSM>
		<Code>E-3D</Code>
		<EuclideanType>ThreeDimensions</EuclideanType>
	</EuclideanSM>
	<GGDMSM>
		<Code>GGeo-3D-2</Code>
		<GGDMSMType>ThreeDimensions</GGDMSMType>
		<Alpha>2</Alpha>
	</GGDMSM>
	<GGDMSM>
		<Code>GGeo-3D-1</Code>
		<GGDMSMType>ThreeDimensions</GGDMSMType>
		<Alpha>1</Alpha>
	</GGDMSM>
	<GGDMSM>
		<Code>GGeo-3D-4</Code>
		<GGDMSMType>ThreeDimensions</GGDMSMType>
		<Alpha>4</Alpha>
	</GGDMSM>
	<Xu17SM>
		<Code>X17-2</Code>
		<Alpha>2</Alpha>
	</Xu17SM>
	<Xu17SM>
		<Code>X17-0.5</Code>
		<Alpha>0.5</Alpha>
	</Xu17SM>
	<Xu19SM>
		<Code>X19</Code>
	</Xu19SM>
	<Xu21SM>
		<Code>X21</Code>
	</Xu21SM>
	<CCSM>
		<Code>CC</Code>
	</CCSM>
	<ChSM>
		<Code>Ch</Code>
	</ChSM>
	<HKSM>
		<Code>HK</Code>
	</HKSM>
	<HY15SM>
		<Code>HY15</Code>
	</HY15SM>
	<HY16SM>
		<Code>HY16</Code>
	</HY16SM>
	<HY16SM>
		<Code>HY16</Code>
	</HY16SM>
	<BASM>
		<Code>BA-1-2</Code>
		<P>1</P>
		<S>2</S>
	</BASM>
	<BASM>
		<Code>BA-2-4</Code>
		<P>2</P>
		<S>4</S>
	</BASM>
	<N26SM>
		<Code>N26</Code>
	</N26SM>
	<XY19SM>
		<Code>XY19</Code>
	</XY19SM>
	<CosineSM>
		<Code>COS</Code>
	</CosineSM>
	<SK1SM>
		<Code>SK1-2D</Code>
		<SK1Type>TwoDimensions</SK1Type>
	</SK1SM>
	<SK2SM>
		<Code>SK2-2D</Code>
		<SK2Type>TwoDimensions</SK2Type>
	</SK2SM>
	<SK3SM>
		<Code>SK3-2D</Code>
		<SK3Type>TwoDimensions</SK3Type>
	</SK3SM>
	<SK4SM>
		<Code>SK4-2D</Code>
		<SK4Type>TwoDimensions</SK4Type>
	</SK4SM>
	<XVBSM>
		<Code>XVB-0-0.05</Code>
		<Alpha>0</Alpha>
		<Wide>0.05</Wide>
		<MiddleMarkWeight>1</MiddleMarkWeight>
		<UpMarkWeight>0.01</UpMarkWeight>
		<DownMarkWeight>0.01</DownMarkWeight>
		<K>5</K>
	</XVBSM>
	<XVBSM>
		<Code>XVB-0-0.1</Code>
		<Alpha>0</Alpha>
		<Wide>0.1</Wide>
		<MiddleMarkWeight>1</MiddleMarkWeight>
		<UpMarkWeight>0.01</UpMarkWeight>
		<DownMarkWeight>0.01</DownMarkWeight>
		<K>5</K>
	</XVBSM>
	<XVBrSM>
		<Code>XVBr-1-10</Code>
		<Alpha>1</Alpha>
		<K>10</K>
	</XVBrSM>
	<XVBrSM>
		<Code>XVBr-0.5-10</Code>
		<Alpha>0.5</Alpha>
		<K>10</K>
	</XVBrSM>
	<XVBrSM>
		<Code>XVBr-0-10</Code>
		<Alpha>0</Alpha>
		<K>10</K>
	</XVBrSM>
	<XVBrSM>
		<Code>XVBr-1-5</Code>
		<Alpha>0.5</Alpha>
		<K>5</K>
	</XVBrSM>
	<XVBrSM>
		<Code>XVBr-0.5-5</Code>
		<Alpha>0.5</Alpha>
		<K>5</K>
	</XVBrSM>
	<XVBrSM>
		<Code>XVBr-0-5</Code>
		<Alpha>0</Alpha>
		<K>5</K>
	</XVBrSM>
	<XVBrSM>
		<Code>XVBr-1</Code>
		<Alpha>1</Alpha>
	</XVBrSM>
	<XVBrSM>
		<Code>XVBr-0.5</Code>
		<Alpha>0.5</Alpha>
	</XVBrSM>
	<XVBrSM>
		<Code>XVBr-0</Code>
		<Alpha>0</Alpha>
	</XVBrSM>
</Measures>
...
```

#### Reporting process
To build a graphical report of the resulting comparisons, you must indicate the path of the file having the results and some report preferences. After that, you can run the process by means of *IFSSimReporter*. To do so you can enter the command

```bash
$ ./IFSSimReporter.exe experiment-configuration.xml
```
where `experiment-configuration.xml` is a file containing the configuration of the process -- which can be the same file used during the previous processes.

The parameters used during the reporting process can be specified as indicated next.

```xml
<?xml version="1.0" encoding="utf-8"?>
<ExperimentConfig xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <BuildingProcessParams/>
  <ComparisonProcessParams/>
  <ReportingProcessParams>
    <!-- This parameter should have the value specified in `ComparisonProcessParams/SimilaritiesFile` -->
    <SimilaritiesFile>/home/your-user/output/IFSMetrics/Similarities/similarities.csv</SimilaritiesFile>
    <!-- This parameter should have the value specified in `ComparisonProcessParams/ReferencesFile` -->
    <ReferencesFile>/home/your-user/output/IFSMetrics/Similarities/references.xml</ReferencesFile>
    <!-- This parameter could have the value specified in `ComparisonProcessParams/SimilaritiesFile` -->
    <WorkingDir>/home/your-user/output/IFSMetrics/Temp/</WorkingDir>

    <ReferentSimilarityMeasure>AoD</ReferentSimilarityMeasure>
    <GNUPlotExecFile>/usr/bin/gnuplot</GNUPlotExecFile>
    <PDFLatexExecFile>/usr/bin/pdflatex</PDFLatexExecFile>  
    <ReportsDir>/home/your-user/output/IFSMetrics/Reports/</ReportsDir>
    <ChartsFileFormat>pdf</ChartsFileFormat>
    <TablesFileFormat>latex</TablesFileFormat>
    <!--<Report>mIndices_AllCategoriesTable</Report>-->
    <!--<Report>SimVsOppLinearModel_AllCategoriesChart</Report>-->
    <!--<Report>SimVsOppLinearModel_SpecificCategoryChart</Report>-->
    <Report>ConsolidatedReport</Report>
    <mIndexReferentMeasure>AoD</mIndexReferentMeasure>
  </ReportingProcessParams>
  <WorkingDir/>
</ExperimentConfig>
```
After running the reporting process, you can find the report(s) in the directory specified in `ReportsDir`.
