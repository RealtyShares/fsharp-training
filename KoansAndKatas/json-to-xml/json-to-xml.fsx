System.Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

// download data set from here and save and unzip. Copy "enron.json" to the script folder
// http://jsonstudio.com/wp-content/uploads/2014/02/enron.zip

#r "../../packages/Newtonsoft.Json/lib/net45/Newtonsoft.Json.dll"

open System
open System.IO

open Newtonsoft.Json

// define a DTO to represent a json email record
type EmailRecord = 
    { sender : string
      recipients : string array
      cc : string array
      bcc : string array
      text : string
      date : DateTime
      subject : string }

let emails = 
    File.ReadAllLines("enron.json")
    |> Seq.map (fun line -> JsonConvert.DeserializeObject<EmailRecord> line)
    |> Seq.toArray