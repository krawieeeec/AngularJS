# moodies.pl

<p>This branch is responsible for client-side of moodies application. Client-side is written in angularJS 1.5.8 Below are contents file of <b>README.md</b> divided on sections.</p>

<!-------------------------------->
<!-- Section of Name Convention -->

# Name Convention
<p>In this project was used several convention depend on used element's angularJS in order better organisation files and developing project for new features. Below, this was listed in table: </br>
<em>Important comment!</em></br>
<em>'<b>PascalCase</b>' is called also '<b>UpperCamelCase</b>'. The same is in case '<b>CamelCase</b>' of conventon. It was named as '<b>lowerCamelCase</b>'. This is important because of used diffrent name inside many tutorials.</em>  
</p>

<table>
  <tr>
    <th>
      Angular's Element
    </th>
    <th>
      Example
    </th>
    <th>
      Applied Convention
    </th>
  </tr>
  <tr>
    <td>
      <em>Controllers</em>
    </td>
    <td>
      <em>Functionality + 'CtrL'</em>
    </td>
    <td>
      <em>PascalCase</em>
    </td>
  </tr>
  <tr>
    <td>
      <em>Modules</em>
    </td>
    <td>
      <em>nameOfModule</em>
    </td>
    <td>
      <em>CamelCase</em>
    </td>
  </tr>
  <tr>
    <td>
      <em>Services</em>
    </td>
    <td>
      <em>NameOfService</em>
    </td>
    <td>
      <em>PascalCase</em>
    </td>
  </tr>
  <tr>
    <td>
      <em>Directives</em>
    </td>
    <td>
      <em>nameOfDirective</em>
    </td>
    <td>
      <em>CamelCase</em>
    </td>
  </tr>
  <tr>
    <td>
      <em>Factories</em>
    </td>
    <td>
      <em>nameOfFactory</em>
    </td>
    <td>
      <em>CamelCase</em>
    </td>
  </tr>
  
</table>
<p><em>Note!</em></br>
  <em>In case of using <b>directives</b> in <b>HTML</b> tags or standalone elements' of <b>components</b> there were applied <b>kebab-case</b> name convention. For example:</br> <code> &lttd attribute=name-directive&gt&lt/td&gt or &ltname-component&gt&lt/name-component&gt</code>.</br> Where next added prefix is separate dash from rest part 'name-directive'.  </em> 
</p>

<!-------------------------------->
<!-- Section of Structure Files -->
# Structure files
<p>Here are described how was divided <em>client-side</em> on structure files:</p>  

| <em>Client</em> </br>
--| <em>app</em> </br>
----| <em>components</em> </br>
----| <em>controllers</em> </br>
----| <em>directives</em> </br>
----| <em>events</em> </br>
----| <em>filters</em> </br>
----| <em>layout</em> </br>
----| <em>partials</em> </br>
----| <em><b>app.module.js</b></em> </br>
--| <em>assets</em> </br>
----| <em>css</em> </br>
----| <em>fonts</em> </br>
----| <em>img</em> </br>
----| <em>lib</em> </br>
-------|<em>angular</em></br>
-------|<em>bootstrap</em></br>
-------|<em>bower_components</em> </br>
----| <em>scripts</em> </br>
--| <em><b>index.html</b></em> </br>
| <em>documentation</em> </br>

<p><em>Note!</br>
During creating structure file in each other folder was added files which were called '_treesub.txt' because of problems with pushing empty folder on remote branch. It was recomennded during creating next empty folders which will use in future work.</em></p>

<h4>Description Folders:</h4>
<dl>
  <dt>Client/app/..</dt>
  <dd> This folder consist of angular app. The intention app is that deliver funcionality to end-user using modern browsers and any devices which access to internet. Client-side written in AngularJS is <b>repsonsive</b> and divided on <b>conponents</b>. This is the most important thing so as to writed script would be can injected to existing app without any problems thanks to <b>dependency injection</b> pattern. Finally that mentioned folder have others significant subfolders among others '<em>templates</em>' and '<em>controller</em>' which each other create <b>components</b> which are injected to <b>partial views</b>. They are placed in '<em>partials</em>' and finally partial views are showed in main layout called '<em>index.html</em>'. All project was designed as <b>SPA(Single Page Application)</b> takeing adventage of dividing project on modules which consist on components therebay they are easily developed and improved.  </dd>
  <dt>Client/assets/..</dt>
  <dd><p>There are placed external resources among others images, fonts and custom css placed respectively in separate folders called the same as points names in given order. At the end folder '<em>lib</em>' have libraries and extension which extend app. Also tere are placed <b>bower_components</b>. It is recomended that further received components were downloaded to '<em>bower_components</em>' folder. The best way that download bower components is writing appropriate command to console.</p> </dd>
  <dt>Client/scripts/..</dt>
  <dd><p>There are placed external and custom scripts JS. </p> </dd>
  <dt>documentation</dt>
  <dd>This folder consist with description of API written in ASP.NET. Description of API has format <em>.pdf</em></dd>
 
</dl>

<h4>Description files:</h4>
<dl>
   <dt>Client/app/app.module.js</dt>
   <dd>This files contain with mainly module of app. In that module are injected dependecies among others embedded Angular modules or external bower components which are used in every place. Every mentioned places are files which are any custom Angular element written by programmer and placed separately and injected(<em>links to</em>) to module. </dd>
   <dt>Client/index.html</dt>
   <dd>This is main layout of app in which are showed <b>partial views</b>. This is place in which occur <b>angularJS routing</b>  </dd>
 </dl>

<!-- Secton of Controllers -->
#Controllers
<p>Code in controllers are written in intended way in order easier further finding intersted part of controller responsible for any functionality. Controllers and others elements in project are written in separate files. As the name suggests folder consists of common controllers which can be add to each components: </p>

<pre><code>angular.module('...').controller('...'){
<em>{{Declaration Methods}}</em>
  $scope.NameOfMethods = {
    //...
  };
<em>{{Declaration Variables}}</em>
  var foo = {
    //...
  };
<em>{{Declaration Rest Services}}</em>
  $http.get('...'){
    //...
  });
<em>{{Declaration Functions}}</em>
  $scope.nameOfFunction = {
    //...
  };
</code></pre>
<p><em>Note!</br>
Above listing declaration of mehtods are used in angularJS' data model.
</em></p>
<!---------------------------->
<!-- Sections of Components -->
#Components
<p>All components are placed in folder with the same name but written in lower case. This app consist of only components which is
responsible for only one any functionality. Thanks to components code is more clearly and orderly what is possible to manage easly all app or writing new functionality finally extend it. That structure app allow us to use pattern SPA(Single Page Application). SPA is idea to show contents without reload all page in every time when user explore our resources. This improve perfomance and reduce using internet bandwitdh also our server can do much more other things instead sending responses to our users in form whole web sites. </p>
<pre><code>
angular.module(<em>nameOfModule</em>).component(<em>nameOfComponent</em>, function () {
  
  //place for builded angular properties
  //Use Property 'template' if you write small template in component but it's recomended to write it in separate file
  //Use 'templateUrl' if you want attach template from external file with extension .html
  
  controller: //Here we write business logic or attach it from external common controller placed in folder 'controllers'

});
</code></pre>


<em>Note!</em>
<p>Each controller consist of template and controller. It is certain box which glues these elements in one form. It is recomended to write each controller in separate files becouse of your IDE will be able to help you with syntax for example highlights it or correct and show your done errors in syntax so good habit is creating component as showed below:

---|components</br>
-------|<b>foo</b></br>
-------|<b>foo.component.js</b></br>
-------|<b>foo.template.html</b></br>
index.html</br>

<p>As you can see that manner has many advantages for example code is divided on small piecies which you will be able to organize better and extend it. In <em>foo.component.js</em> you should write business logic and attach template from <em>foo.template.html </em> which are responsible for to show data from controller and start interaction with end-user. </p></br>

<!---------------------------->
<!-- Sections of Routing -->
#Routing
In this app will be applied <em>'ui-router'</em> module. Each component will be loading in index.html  according to convention SPA(Single Page Application). Thanks to this module each routing we can treat as state of machine which is loading when user try change adress URL in itself browser or when use hyperlinks in form click button to go to other site in our web service.




<!----------------------------------->
<!-- Sections of Significant links -->
#Significant links


<p>Links to style of conventions's angular scripts</p> <a>https://github.com/mgechev/angularjs-style-guide#naming-conventions</a></br>
Links to routing:
<a>https://github.com/excellalabs/ngComponentRouter</a></br>
<a>https://ui-router.github.io/guide/ng1/route-to-component</a><br>

#Notes

*description all placed files js.</br>
*description how we create module in components which are injected to main module.</br>
*description components and correct organization file.</br>
*description runnging app through firefox browser. If it will be chrome you have to download and configure node.js server(using google technology).</br>
*correct description app/app.module.js this has a little bullshit.</br>
*write new service which will be injected to components in addListView. It will be resposible for creating new list.</br>
*Differents http://stackoverflow.com/questions/21023763/angularjs-difference-between-angular-route-and-angular-ui-router
*Very usefull links: http://bguiz.github.io/js-standards/angularjs/application-structure-lift-principle/

<small><em>Dawid Krawczyk, 2016</em></small>
