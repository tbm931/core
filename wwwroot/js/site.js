const uri = '/Book';
let books = [];

document.addEventListener("DOMContentLoaded", function () {
    if (localStorage.getItem("token") === 'undefined' || localStorage.getItem("author") === 'undefined' || localStorage.getItem("author") === null)
        document.location.href = "index.html";
    else {
        if (JSON.parse(localStorage.getItem("author")).isAdmin) {
            const arr = document.getElementsByClassName("admin");
            for (let i = 0; i < arr.length; i++) {
                arr[i].style.display = "block";
            }
        }
        document.getElementById('edit-author-name').value = JSON.parse(localStorage.getItem("author")).name;
        document.getElementById('edit-phone').value = JSON.parse(localStorage.getItem("author")).phone;
        document.getElementById('edit-isAdmin').checked = JSON.parse(localStorage.getItem("author")).isAdmin;
    }
});

function getItems() {
    const token = localStorage.getItem("token");
    fetch(uri, {
        method: 'GET',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            "Authorization": `Bearer ${token}`
        },
    }).then(response => response.json())
        .then(data => _displayItems(data))
        .catch(error => console.error('Unable to get items.', error));
}

function addItem() {
    const token = localStorage.getItem("token");
    const addNameTextbox = document.getElementById('add-name');
    const addAuthorNameTextbox = document.getElementById('add-authorName');

    const item = {
        name: addNameTextbox.value.trim(),
        AuthorName: addAuthorNameTextbox.value.trim()
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
        .then(() => {
            getItems();
            addNameTextbox.value = '';
        })
        .catch(error => console.error('Unable to add item.', error));
}

function editItem() {
    const token = localStorage.getItem("token");
    const editNameTextbox = document.getElementById('edit-author-name');
    const editPhoneTextbox = document.getElementById('edit-phone');
    const editIsAdmin = document.getElementById('edit-isAdmin');

    const item = {
        "id": JSON.parse(localStorage.getItem("author")).id,
        "name": editNameTextbox.value.trim(),
        "phone": editPhoneTextbox.value.trim(),
        "isDdmin": editIsAdmin.checked
    };

    fetch(`/Author/${item.id}`, {
        method: 'PUT',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify(item)
    }).then(response => {
        if (response.status < 300) {
            localStorage.setItem("author") = JSON.stringify(item);
            window.location.href = 'second.html';
        } else
            console.error('Unable to edit author.')
    })
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
    const item = books.find(item => item.id === id);
    document.getElementById('edit-name').value = item.name;
    document.getElementById('edit-id').value = item.id;
    document.getElementById('edit-authorname').value = item.authorName;
    document.getElementById('editForm').style.display = 'block';
}

async function updateItem() {
    const token = localStorage.getItem("token");
    const itemId = document.getElementById('edit-id').value;
    const item = {
        Id: parseInt(itemId, 10),
        Name: document.getElementById('edit-name').value,
        AuthorName: document.getElementById('edit-authorname').value
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
    const name = (itemCount === 1) ? 'book' : 'books';

    document.getElementById('counter').innerText = `${itemCount} ${name}`;
}

function _displayItems(data) {
    const tBody = document.getElementById('books');
    tBody.innerHTML = '';

    _displayCount(data.length);

    const button = document.createElement('button');

    data.forEach(item => {
        let editButton = button.cloneNode(false);
        editButton.innerText = 'Edit';
        editButton.setAttribute('onclick', `displayEditForm(${item.id})`);

        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Delete';
        deleteButton.setAttribute('onclick', `deleteItem(${item.id})`);

        let tr = tBody.insertRow();

        let td1 = tr.insertCell(0);
        let textNode1 = document.createTextNode(item.name);
        td1.appendChild(textNode1);

        let td2 = tr.insertCell(1);
        let textNode2 = document.createTextNode(item.authorName);
        td2.appendChild(textNode2);

        let td3 = tr.insertCell(2);
        td3.appendChild(editButton);

        let td4 = tr.insertCell(3);
        td4.appendChild(deleteButton);
    });

    books = data;
}