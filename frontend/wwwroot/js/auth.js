document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("loginForm");
    if (form) {
        form.addEventListener("submit", async function (e) {
            e.preventDefault();
            const email = document.getElementById("email").value;
            const password = document.getElementById("password").value;

            const response = await fetch("https://votre-api.com/api/auth/login", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ email, password })
            });

            if (response.ok) {
                const data = await response.json();
                localStorage.setItem("token", data.token);
                localStorage.setItem("role", data.role);
                window.location.href = "/Redirection";
            } else {
                alert("Email ou mot de passe incorrect.");
            }
        });
    }

    // Affichage des menus selon rôle
    const role = localStorage.getItem("role");
    if (role) {
        if (role === "Admin") document.getElementById("menu-admin")?.classList.remove("d-none");
        if (role === "Enseignant") document.getElementById("menu-enseignant")?.classList.remove("d-none");
        if (role === "Etudiant") document.getElementById("menu-etudiant")?.classList.remove("d-none");
    }
});
