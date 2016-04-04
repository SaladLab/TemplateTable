#I @"packages/FAKE/tools"
#I @"packages/FAKE.BuildLib/lib/net451"
#r "FakeLib.dll"
#r "BuildLib.dll"

open Fake
open BuildLib

let solution = 
    initSolution
        "./TemplateTable.sln" "Release" 
        [ { // Core Libraries
            emptyProject with Name = "TemplateTable"
                              Folder = "./core/TemplateTable" }
          { // Plugin Libraries
            emptyProject with Name = "TemplateTable.Json"
                              Folder = "./plugins/TemplateTable.Json"
                              Dependencies = 
                                  [ ("TemplateTable", "")
                                    ("Newtonsoft.Json", "") ] }
          { emptyProject with Name = "TemplateTable.Protobuf"
                              Folder = "./plugins/TemplateTable.Protobuf"
                              Dependencies = 
                                  [ ("TemplateTable", "")
                                    ("protobuf-net", "") ] } ]

Target "Clean" <| fun _ -> cleanBin

Target "AssemblyInfo" <| fun _ -> generateAssemblyInfo solution

Target "Restore" <| fun _ -> restoreNugetPackages solution

Target "Build" <| fun _ -> buildSolution solution

Target "Test" <| fun _ -> testSolution solution

Target "Cover" <| fun _ ->
    coverSolutionWithParams 
        (fun p -> { p with Filter = "+[TemplateTable*]* -[*.Tests]*" })
        solution
    
Target "Coverity" <| fun _ -> coveritySolution solution "SaladLab/TemplateTable"

Target "Nuget" <| fun _ ->
    createNugetPackages solution
    publishNugetPackages solution

Target "CreateNuget" <| fun _ ->
    createNugetPackages solution

Target "PublishNuget" <| fun _ ->
    publishNugetPackages solution

Target "Unity" <| fun _ -> buildUnityPackage "./core/UnityPackage"

Target "CI" <| fun _ -> ()

Target "Help" <| fun _ -> 
    showUsage solution (fun name -> 
        if name = "unity" then Some("Build UnityPackage", "")
        else None)

"Clean"
  ==> "AssemblyInfo"
  ==> "Restore"
  ==> "Build"
  ==> "Test"

"Build" ==> "Nuget"
"Build" ==> "CreateNuget"
"Build" ==> "Cover"
"Restore" ==> "Coverity"

"Test" ==> "CI"
"Cover" ==> "CI"
"Nuget" ==> "CI"

RunTargetOrDefault "Help"
