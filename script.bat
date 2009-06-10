chdir /d %1

Dependencies\JSMin.exe Kayjax\Script\Lib\JQuery Kayjax\Script\Lib\jquery-1.3.2.js false
Dependencies\JSMin.exe Kayjax\Script\Src\KayjaxBase.js Kayjax\Script\Src\KayjaxPrototype.js Kayjax\Script\kayjax-prototype-1.0.9.js %2
Dependencies\JSMin.exe Kayjax\Script\Src\KayjaxBase.js Kayjax\Script\Src\KayjaxJQuery.js Kayjax\Script\kayjax-jquery-1.0.9.js %2

copy Kayjax\Script\Lib\prototype-1.6.0.3.js Test\prototype-1.6.0.3.js
copy Kayjax\Script\Lib\jquery-1.3.2.js Test\jquery-1.3.2.js
copy Kayjax\Script\kayjax-prototype-1.0.9.js Test\kayjax-prototype-1.0.9.js
copy Kayjax\Script\kayjax-jquery-1.0.9.js Test\kayjax-jquery-1.0.9.js