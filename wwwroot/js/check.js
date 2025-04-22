const checkAuthor = async () => {
    event.preventDefault();
    const id = document.getElementById("id");
    const name1 = document.getElementById("name");
    const loginRequest = {
        "Id": id.value,
        "Name": name1.value
    };
    const authorRequest = {
        ifDo: false,
        token: ""
    }
    try {
        let response;
        const token = await fetch("https://localhost:7011/Author/Login", {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(loginRequest)
        }).then(res => {
            response = res;
            if (!res.ok) {
                throw new Error(`HTTP error! Status: ${res.status}`);
            }
            return response.json();
        }).then(res => {
            return res;
        })
        localStorage.setItem("token", token);
        if (!response.ok) {
            throw new Error('Login failed');
        }
        authorRequest.ifDo = true;
        authorRequest.token = token;
        const author = await fetch("https://localhost:7011/Author/GetAuthorFromT", {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(authorRequest)
        }).then(res => {
            response = res;
            if (!res.ok) {
                throw new Error(`HTTP error! Status: ${res.status}`);
            }
            return response.text();
        })
        localStorage.setItem("author", author);
        alert("נכנסתם בהצלחה!!!");
        window.location.href = "second.html";

    } catch (error) {
        alert("Unable to get in: " + error.message);
    }
};