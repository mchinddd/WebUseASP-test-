document.addEventListener("DOMContentLoaded", function () {
    // Sidebar elements
    const sidebar = document.querySelector(".sidebar");

    // Hàm toggle sidebar
    function toggleSidebar() {
        if (sidebar) {
            sidebar.classList.toggle("collapsed");
        }
    }

    if (sidebar) {
        sidebar.addEventListener("click", function (e) {
            const isLink = e.target.closest('a');
            if (!isLink &&
                !e.target.classList.contains("menu-item") &&
                e.target.tagName !== "SPAN" &&
                e.target.tagName !== "I" &&
                !sidebar.classList.contains("collapsed")
            ) {
                sidebar.classList.add("collapsed");
            } else {
                sidebar.classList.remove("collapsed");
            }
        });

        document.addEventListener("click", (e) => {
            if (
                window.innerWidth < 992 &&
                !sidebar.contains(e.target) &&
                !sidebar.classList.contains("collapsed")
            ) {
                sidebar.classList.add("collapsed");
            }
        });
    }

    const themeToggle = document.getElementById("themeToggle");
    if (themeToggle) {
        themeToggle.addEventListener("click", function () {
            document.body.classList.toggle("dark-mode");
            const icon = themeToggle.querySelector("i");
            if (document.body.classList.contains("dark-mode")) {
                icon.classList.remove("fa-moon");
                icon.classList.add("fa-sun");
            } else {
                icon.classList.remove("fa-sun");
                icon.classList.add("fa-moon");
            }
            localStorage.setItem(
                "theme",
                document.body.classList.contains("dark-mode") ? "dark" : "light"
            );
        });
        const savedTheme = localStorage.getItem("theme");
        if (savedTheme === "dark") {
            document.body.classList.add("dark-mode");
            themeToggle.querySelector("i").classList.remove("fa-moon");
            themeToggle.querySelector("i").classList.add("fa-sun");
        }
    }

    // Simulate login state
    const userAvatar = document.querySelector(".user-avatar");
    const isLoggedIn = true;
    if (!isLoggedIn && userAvatar) {
        userAvatar.innerHTML =
            '<i class="fas fa-user-circle" style="font-size: 45px; color: #666;"></i>';
    }

    // Đóng sidebar khi click ra ngoài (trên mobile)
    document.addEventListener("click", (e) => {
        if (
            window.innerWidth < 992 &&
            sidebar &&
            !sidebar.contains(e.target) &&
            !sidebar.classList.contains("collapsed")
        ) {
            toggleSidebar();
        }
    });
});


// Thay đổi màu chủ đề
document.querySelectorAll(".color-option").forEach((option) => {
    option.addEventListener("click", function () {
        document.querySelectorAll(".color-option").forEach((opt) => {
            opt.classList.remove("active");
        });
        this.classList.add("active");

        const color = this.style.backgroundColor;
        document.documentElement.style.setProperty("--primary", color);
    });
});

// Thay đổi chế độ hiển thị
document.querySelectorAll(".theme-option").forEach((option) => {
    option.addEventListener("click", function () {
        document.querySelectorAll(".theme-option").forEach((opt) => {
            opt.classList.remove("active");
        });
        this.classList.add("active");

        if (this.classList.contains("dark-theme")) {
            document.body.classList.add("dark-mode");
            localStorage.setItem("theme", "dark");
        } else {
            document.body.classList.remove("dark-mode");
            localStorage.setItem("theme", "light");
        }
    });
});

// Phân trang
document.querySelectorAll(".page-btn").forEach((btn) => {
    btn.addEventListener("click", function () {
        document.querySelectorAll(".page-btn").forEach((b) => {
            b.classList.remove("active");
        });
        this.classList.add("active");
    });
});

// Hiển thị modal thêm học sinh
document.querySelector(".add-btn")?.addEventListener("click", function () {
    // Code hiển thị modal thêm học sinh
    alert("Modal thêm học sinh sẽ được hiển thị ở đây");
});
