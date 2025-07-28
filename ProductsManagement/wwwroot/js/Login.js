// تهيئة اللغة عند التحميل
document.addEventListener('DOMContentLoaded', () => {
    const lang = localStorage.getItem('lang') || 'en';
    setLanguage(lang);
});

// تغيير اللغة وحفظها
function toggleLanguage() {
    const currentLang = localStorage.getItem('lang') || 'en';
    const newLang = currentLang === 'en' ? 'ar' : 'en';
    localStorage.setItem('lang', newLang);
    setLanguage(newLang);
}

// تطبيق اللغة على الصفحة
function setLanguage(lang) {
    const html = document.getElementById('html-root');
    html.lang = lang;
    html.dir = lang === 'ar' ? 'rtl' : 'ltr';

    document.getElementById('lang-label').textContent = lang.toUpperCase();

    const translations = {
        en: {
            appName: 'Products Management System',
            loginTitle: 'Admin Login',
            email: 'Email',
            password: 'Password',
            login: 'Login'
        },
        ar: {
            appName: 'لوحة التحكم',
            loginTitle: 'تسجيل دخول المشرف',
            email: 'البريد الإلكتروني',
            password: 'كلمة المرور',
            login: 'تسجيل الدخول'
        }
    };

    const t = translations[lang];
    document.getElementById('app-name').textContent = t.appName;
    document.getElementById('login-title').textContent = t.loginTitle;
    document.getElementById('email-label').textContent = t.email;
    document.getElementById('password-label').textContent = t.password;
    document.getElementById('login-button').textContent = t.login;
}

// إرسال بيانات الدخول
async function loginAdmin(event) {
    event.preventDefault();

    const email = document.getElementById("email").value;
    const password = document.getElementById("password").value;
    const errorMsg = document.getElementById("error-message");

    const loginData = {
        email: email,
        password: password
    };

    try {
        const response = await fetch('/api/Account/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(loginData)
        });

        const data = await response.json();

        if (response.ok) {
            sessionStorage.setItem('token', data.token);
            sessionStorage.setItem('role', data.role);
            if (data.role == "admin") {
                //console.log("Admin login successful");
                window.location.replace('/admin/dashboard');
            }
            else console.log("Not admin");
        } else {
            errorMsg.textContent = data.errors?.[0] || 'Login failed';
        }
    } catch (error) {
        errorMsg.textContent = 'Network error, please try again.';
        console.error('Error:', error);
    }
    document.addEventListener('DOMContentLoaded', () => {
        const role = sessionStorage.getItem('role');
        if (role !== 'admin') {
            window.location.href = '/admin/login';
        }

        document.getElementById('current-role').textContent = `Role: ${role}`;
    });
}

