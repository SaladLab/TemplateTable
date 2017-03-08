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
                              PackagePrerelease = "beta"
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

Target "PackNuget" <| fun _ -> createNugetPackages solution

Target "PackUnity" <| fun _ ->
    packUnityPackage "./core/UnityPackage/TemplateTable.unitypackage.json"

Target "Pack" <| fun _ -> ()

Target "PublishNuget" <| fun _ -> publishNugetPackages solution

Target "PublishUnity" <| fun _ -> ()

Target "Publish" <| fun _ -> ()

Target "CI" <| fun _ -> ()

Target "Help" <| fun _ -> 
    showUsage solution (fun _ -> None)

"Clean"
  ==> "AssemblyInfo"
  ==> "Restore"
  ==> "Build"
  ==> "Test"

"Build" ==> "Cover"
"Restore" ==> "Coverity"

let isPublishOnly = getBuildParam "publishonly"

"Build" ==> "PackNuget" =?> ("PublishNuget", isPublishOnly = "")
"Build" ==> "PackUnity" =?> ("PublishUnity", isPublishOnly = "")
"PackNuget" ==> "Pack"
"PackUnity" ==> "Pack"
"PublishNuget" ==> "Publish"
"PublishUnity" ==> "Publish"

"Test" ==> "CI"
"Cover" ==> "CI"
"Publish" ==> "CI"

RunTargetOrDefault "Help"
