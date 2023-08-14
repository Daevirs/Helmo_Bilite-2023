// On cache tout ce qui à un rapport avec les membres à l'activation de la page
let collection = document.getElementsByClassName("Membre")
for(let i = 0; i < collection.length; i++){
    collection.item(i).disabled = true
    collection.item(i).style.display = 'none'
}
document.getElementById('role').value = 'Client'

function switchRole(){
    let currentRole = document.getElementById("role").value
    if(currentRole === "Dispatcher"){
        affichageChauffeur()
    } else {
        affichageDispatcher()
    }
}

function switchFormulaire(){
    if(document.getElementById('switchForm').checked){
        document.getElementById('roleBoutton').checked = false
        affichageMembre()
        document.getElementById('role').value = 'Dispatcher'
    } else {
        affichageClient()
        document.getElementById('role').value = 'Client'
    }
}

function affichageClient(disable = false, display = 'block'){
    let client = document.getElementsByClassName("Client")
    for(let i = 0; i < client.length; i++){
        client.item(i).disabled = disable
        client.item(i).style.display = display
    }
    if(!disable){
        document.getElementById('role').value = "Client"
        affichageMembre(true, 'none')
    }
}

function affichageMembre(disable = false, display = 'block'){
    let membre = document.getElementsByClassName("Membre")
    for(let i = 0; i < membre.length; i++){
        membre.item(i).disabled = disable
        membre.item(i).style.display = display
    }
    if(!disable){
        affichageClient(true, 'none')
        affichageChauffeur(true, 'none')
        affichageDispatcher()
    }
}

function affichageDispatcher(disable = false, display = 'block'){
    let dispatcher = document.getElementsByClassName("Dispatcher")
    for(let i = 0; i < dispatcher.length; i++){
        dispatcher.item(i).disabled = disable
        dispatcher.item(i).style.display = display
    }
    if(!disable){
        document.getElementById('role').value = "Dispatcher"
        affichageChauffeur(true, 'none')
    }
}

function affichageChauffeur(disable = false, display = 'block'){
    let chauffeur = document.getElementsByClassName("Chauffeur")
    for(let i = 0; i < chauffeur.length; i++){
        chauffeur.item(i).disabled = disable
        chauffeur.item(i).style.display = display
    }
    if(!disable){
        document.getElementById('role').value = "Chauffeur"
        affichageDispatcher(true, 'none')
    }
}