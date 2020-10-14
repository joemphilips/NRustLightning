// based on: https://github.com/lucamug/elm-form-examples/blob/master/src/Example_18.elm
module NRustLightning.GUI.Client.Utils

open Bolero.Html

let exampleComment =
    Map.ofList
        [
             ("index", "Examples of Form built in elm.")
             ("1" , "First version: just an old simple form.")
             ("2" , "Changed the form to be à la Elm using \"application/x-www-form-urlencoded\" as encoding system")
             ("3" , "Changed the encoding system to json")
             ("4" , "Added validation")
             ("5" , "Moved the field updates out of the update function")
             ("6" , "Replaced the <form> element with <div> and added \"onClick SubmitForm\" to the button")
             ("7" , "Restored the \"submit-on-enter\" behavior")
             ("8" , "Added validation while typing")
             ("9" , "Created the helper \"viewInput\" that generalized the creation of input fields")
             ("10" , "Adding \"showErrors\" functionality that show error only after the first submit ")
             ("11" , "Adding focus detection so that focus is evident also during history playback")
             ("12" , "Adding the icon to hide and show the password")
             ("13" , "Adding a spinner while the app is waiting for an answer")
             ("14" , "Adding \"Floating Label\"")
             ("15" , "Adding Checkboxes")
             ("16" , "Encoded Checkboxes values into the Json for sending to the server")
             ("17" , "Adding maximum number of checkable fruits")
             ("18" , "Adding svg fruit icons")
             ("19" , "Adding a date picker")
             ("20" , "Adding HTML date")
             ("21" , "Adding Autocomplete field")
        ]

let viewFooter version =
    div
        [ attr.classes ["footer"] ]
        [
            a [ attr.href "https://github.com/lucamug/elm-form-examples" ] [  text " [ code ] " ]
            a [ attr.href "https://medium.com/@l.mugnaini/forms-in-elm-validation-tutorial-and-examples-2339830055da" ] [ text " [ article ]" ]
        ]
let urlMirrorService = "https://httpbin.org/post"
    
let getComment version =
    Map.tryFind version exampleComment
    |> Option.defaultValue ""

let viewHeader version =
    div
        [ attr.classes ["header"] ]
        [
            h1 [] [text ("Elm Form - Example" + version)]
            p [] [ text <| (getComment version) ]
        ]
        
let viewSimple exampleVersion viewForm =
    div
        []
        [ viewHeader exampleVersion
          viewForm
          viewFooter exampleVersion
           ]
        
let viewResponse response =
    div
        [ attr.classes ["response-container"] ]
        [ h2 [] [ text "Response" ] ]
let inline viewUtils model (modelResponse: _ option) exampleVersion viewForm =
    div
        []
        [
            viewHeader exampleVersion
            viewForm model
            cond modelResponse <| function
                                  | Some resp -> viewResponse  resp
                                  | None -> empty
        ]
let view = viewUtils

