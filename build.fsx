// --------------------------------------------------------------------------------------
// FAKE build script
// --------------------------------------------------------------------------------------

#r @"packages/build-tools/FAKE/tools/FakeLib.dll"

//System.Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

open Fake
open Fake.UserInputHelper

open System
open System.IO
open System.Diagnostics

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

Target "Restore" (fun _ ->
    paketRestore "main"
)

Target "GeneratePaketLoadScripts" (fun _ ->
    let paketPath = (findToolFolderInSubPath "paket.exe" (currentDirectory @@ ".paket")) @@ "paket.exe"
    ProcessHelper.Shell.Exec(paketPath,"generate-load-scripts --framework net461 --type fsx",currentDirectory) |> ignore
)

// --------------------------------------------------------------------------------------
// Run all targets by default. Invoke 'build <Target>' to override

Target "All" DoNothing

"Restore"
  ==> "GeneratePaketLoadScripts"
  ==> "All"

RunTargetOrDefault "All"