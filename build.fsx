// --------------------------------------------------------------------------------------
// FAKE build script
// --------------------------------------------------------------------------------------

#r @"packages/build-tools/FAKE/tools/FakeLib.dll"

//System.Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

open Fake
open Fake.AssemblyInfoFile
open Fake.UserInputHelper

open System
open System.IO
open System.Diagnostics

// --------------------------------------------------------------------------------------
// START TODO: Provide project-specific details below
// --------------------------------------------------------------------------------------

// Information about the project are used
//  - for version and project name in generated AssemblyInfo file

// The name of the project
// (used by attributes in AssemblyInfo, name of a NuGet package and directory in 'src')
let project = "fsharp-training"

// Short summary of the project
// (used as description in AssemblyInfo and as a short summary for NuGet package)
let summary = "F# training repo"

// Longer description of the project
// (used as a description for NuGet package; line breaks are automatically cleaned up)
let description = summary

// List of author names (for NuGet package)
let authors = [ "RealtyShares, Inc" ]

// File system information
let solutionFile  = "FSharpTraining/FSharpTraining.sln"

// define test executables
let testExecutables = "tests/**/bin/Debug/*Tests*.exe"

// Git configuration (used for publishing documentation in gh-pages branch)
// The profile where the project is posted
let gitOwner = "RealtyShares"
let gitHome = "https://github.com/" + gitOwner

// The name of the project on GitHub
let gitName = "fsharp-training"

// The url for the raw files hosted
let gitRaw = environVarOrDefault "gitRaw" "https://raw.github.com/RealtyShares"

// Disable writing to default Fake.Errors.txt files, which causes resource contention while multiple jenkins processes are running
MSBuildLoggers <- []

// --------------------------------------------------------------------------------------
// END TODO: The rest of the file includes standard build steps
// --------------------------------------------------------------------------------------

[<AutoOpen>]
module private Helpers = 
    // a custom version of Fake.Paket.Restore that uses the new --group paket cli param
    let paketRestore group =
        let parameters = Paket.PaketRestoreDefaults()
        use __ = traceStartTaskUsing "PaketRestore" parameters.WorkingDir

        let restoreResult =
            ExecProcess (fun info ->
                info.FileName <- parameters.ToolPath
                info.WorkingDirectory <- parameters.WorkingDir
                info.Arguments <- sprintf "restore --group %s" group ) parameters.TimeOut

        if restoreResult <> 0 then failwithf "Error during restore %s." parameters.WorkingDir

    let cleanBuildArtifacts() =
        [!! "FSharpTraining/**/bin"; !! "FSharpTraining/**/obj";
         !! "tests/**/bin"; !! "tests/**/obj"]
        |> Seq.collect id
        |> CleanDirs

// Generate assembly info files with the right version & up-to-date information
Target "AssemblyInfo" (fun _ ->
    let getAssemblyInfoAttributes projectName =
        [ Attribute.Title (projectName)
          Attribute.Product project
          Attribute.Description summary ]

    let getProjectDetails projectPath =
        let projectName = System.IO.Path.GetFileNameWithoutExtension(projectPath)
        ( projectPath,
          projectName,
          System.IO.Path.GetDirectoryName(projectPath),
          (getAssemblyInfoAttributes projectName)
        )

    [!! "FSharpTraining/**/*.??proj"; !! "tests/**/*.??proj"]
    |> Seq.collect id
    |> Seq.map getProjectDetails
    |> Seq.iter (fun (projFileName, projectName, folderName, attributes) ->
        match projFileName with
        | Fsproj -> CreateFSharpAssemblyInfo (folderName @@ "AssemblyInfo.fs") attributes
        | Csproj -> CreateCSharpAssemblyInfo ((folderName @@ "Properties") @@ "AssemblyInfo.cs") attributes
        | Vbproj -> CreateVisualBasicAssemblyInfo ((folderName @@ "My Project") @@ "AssemblyInfo.vb") attributes)
)

Target "Restore" (fun _ ->
    paketRestore "main"
)

Target "GeneratePaketLoadScripts" (fun _ ->
    let paketPath = (findToolFolderInSubPath "paket.exe" (currentDirectory @@ ".paket")) @@ "paket.exe"
    ProcessHelper.Shell.Exec(paketPath,"generate-load-scripts --framework net461 --type fsx",currentDirectory) |> ignore
)

// --------------------------------------------------------------------------------------
// Clean build results

Target "Clean" (fun _ ->
    CleanDirs ["bin"; "temp"]
)

Target "CleanBuildArtifacts" cleanBuildArtifacts

Target "Debug" (fun _ ->
    if hasBuildParam "Clean"
        then cleanBuildArtifacts()

    MSBuild
        ""
        "Build"
        ([
            ("Configuration", "Debug")
            ("Verbosity", "minimal")
            #if MONO
            ("DefineConstants", "MONO")
            #endif
        ])
        (!! solutionFile)
        |> ignore
)

// --------------------------------------------------------------------------------------
// Run all targets by default. Invoke 'build <Target>' to override

Target "All" DoNothing

"Restore"
  ==> "GeneratePaketLoadScripts"
  ==> "Debug"
  ==> "All"

RunTargetOrDefault "All"