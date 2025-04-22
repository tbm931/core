const uri = '/Author';
let authors = [];

if (localStorage.getItem("token") === 'undefined' || localStorage.getItem("author") === 'undefined' || localStorage.getItem("author") === null)
    document.location.href = "index.html";

function getItems() {
    const token = localStorage.getItem("token");
    fetch(uri, {
        method: 'GET',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            "Authorization": `Bearer ${token}`
        },
    })

        .then(response => response.json())
        .then(data => _displayItems(data))
        .catch(error => console.error('Unable to get items.', error));
}

function addItem() {
    const token = localStorage.getItem("token");
    const addIdTextbox = document.getElementById('add-id');
    const addNameTextbox = document.getElementById('add-name');
    const addPhoneTextbox = document.getElementById('add-phone');
    const addAdminTextbox = document.getElementById('add-isAdmin');

    const item = {
        id: addIdTextbox.value,
        name: addNameTextbox.value.trim(),
        Phone: addPhoneTextbox.value.trim(),
        IsAdmin: addAdminTextbox.checked
    };

    fetch(uri, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify(item)
    })
        .then(response => response.json())
        .then(data => {
            getItems();
            addNameTextbox.value = '';
        })
        .catch(error => console.error('Unable to add item.', error));
}

function editItem() {
    const token = localStorage.getItem("token");
    const editNameTextbox = document.getElementById('edit-name');
    const editPhoneTextbox = document.getElementById('edit-phone');
    const editIsAdmin = document.getElementById('edit-isAdmin');

    const item = {
        id: JSON.parse(localStorage.getItem("author")).id,
        name: editNameTextbox.value.trim(),
        phone: editPhoneTextbox.value.trim(),
        isAdmin: editIsAdmin.value
    };

    fetch(`/Author/${item.id}`, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify(item)
    })
        .then(response => response.json())
        .then(() => {
            window.location.href = 'second.html';
        })
        .catch(error => console.error('Unable to edit author.', error));
}

function deleteItem(id) {
    const token = localStorage.getItem("token");
    fetch(`${uri}/${id}`, {
        method: 'DELETE',
        headers: {
            "Authorization": `Bearer ${token}`
        }
    })
        .then(() => getItems())
        .catch(error => console.error('Unable to delete item.', error));
}

function displayEditForm(id) {
    const item = authors.find(item => item.id == id);
    document.getElementById('edit-name').value = item.name;
    document.getElementById('edit-id').value = item.id;
    document.getElementById('edit-phone').value = item.phone;
    document.getElementById('edit-isAdmin').checked = item.isAdmin;
    document.getElementById('editForm').style.display = 'block';
}

async function updateItem() {
    const token = localStorage.getItem("token");
    const itemId = document.getElementById('edit-id').value;
    const item = {
        "Id": itemId.toString(),
        "Name": document.getElementById('edit-name').value,
        "Phone": document.getElementById('edit-phone').value,
        "IsAdmin": document.getElementById('edit-isAdmin').checked
    };

    await fetch(`${uri}/${itemId}`, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify(item)
    })
        .then(() => getItems())
        .catch(error => console.error('Unable to update item.', error));

    closeInput();

    return false;
}

function closeInput() {
    document.getElementById('editForm').style.display = 'none';
}

function _displayCount(itemCount) {
    const name = (itemCount === 1) ? 'author' : 'authors';

    document.getElementById('counter').innerText = `${itemCount} ${name}`;
}

function _displayItems(data) {
    const tBody = document.getElementById('authors');
    tBody.innerHTML = '';

    _displayCount(data.length);

    const button = document.createElement('button');

    data.forEach(item => {
        let name = document.createElement('input');
        name.type = 'checkbox';
        name.disabled = true;
        name.checked = item.isAdmin;

        let editButton = button.cloneNode(false);
        editButton.innerText = 'Edit';
        editButton.setAttribute('onclick', `displayEditForm(${item.id})`);

        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Delete';
        deleteButton.setAttribute('onclick', `deleteItem(${item.id})`);

        let tr = tBody.insertRow();

        let td0 = tr.insertCell(0);
        td0.style.display = "none";
        td0.textContent = item.id;

        let td1 = tr.insertCell(1);
        let textNode1 = document.createTextNode(item.name);
        td1.appendChild(textNode1);

        let td2 = tr.insertCell(2);
        let textNode2 = document.createTextNode(item.phone);
        td2.appendChild(textNode2);

        let td3 = tr.insertCell(3);
        td3.appendChild(name);

        let td4 = tr.insertCell(4);
        td4.appendChild(editButton);

        let td5 = tr.insertCell(5);
        td5.appendChild(deleteButton);
    });

    authors = data;
}