function displayDatetime() {
	var currentDate = new Date();
	var datetimeElement = document.getElementById("datetime");
	datetimeElement.innerHTML = currentDate;
}



jQuery(document).ready(function () {
	ImgUpload();
  });
  
  function ImgUpload() {
	var imgWrap = "";
	var imgArray = [];
  
	$('.upload__inputfile').each(function () {
	  $(this).on('change', function (e) {
		imgWrap = $(this).closest('.upload__box').find('.upload__img-wrap');
		var maxLength = $(this).attr('data-max_length');
  
		var files = e.target.files;
		var filesArr = Array.prototype.slice.call(files);
		var iterator = 0;
		filesArr.forEach(function (f, index) {
  
		  if (!f.type.match('image.*')) {
			return;
		  }
  
		  if (imgArray.length > maxLength) {
			return false
		  } else {
			var len = 0;
			for (var i = 0; i < imgArray.length; i++) {
			  if (imgArray[i] !== undefined) {
				len++;
			  }
			}
			if (len > maxLength) {
			  return false;
			} else {
			  imgArray.push(f);
  
			  var reader = new FileReader();
			  reader.onload = function (e) {
				var html = "<div class='upload__img-box'><div style='background-image: url(" + e.target.result + ")' data-number='" + $(".upload__img-close").length + "' data-file='" + f.name + "' class='img-bg'><div class='upload__img-close'></div></div></div>";
				imgWrap.append(html);
				iterator++;
			  }
			  reader.readAsDataURL(f);
			}
		  }
		});
	  });
	});
  
	$('body').on('click', ".upload__img-close", function (e) {
	  var file = $(this).parent().data("file");
	  for (var i = 0; i < imgArray.length; i++) {
		if (imgArray[i].name === file) {
		  imgArray.splice(i, 1);
		  break;
		}
	  }
	  $(this).parent().parent().remove();
	});
  };

  //pagination of table
var tbody = document.querySelector("tbody");
var pageUl = document.querySelector(".pagination");
var itemShow = document.querySelector("#itemperpage");
var tr = tbody.querySelectorAll("tr");
var emptyBox = [];
var index = 1;
var itemPerPage = 8;

for(let i=0; i<tr.length; i++){ emptyBox.push(tr[i]);}

itemShow.onchange = giveTrPerPage;
function giveTrPerPage(){
	itemPerPage = Number(this.value);
	// console.log(itemPerPage);
	displayPage(itemPerPage);
	pageGenerator(itemPerPage);
	getpagElement(itemPerPage);
}

function displayPage(limit){
	tbody.innerHTML = '';
	for(let i=0; i<limit; i++){
		tbody.appendChild(emptyBox[i]);
	}
	const  pageNum = pageUl.querySelectorAll('.list');
	pageNum.forEach(n => n.remove());
}
displayPage(itemPerPage);
function pageGenerator(getem){
	const num_of_tr = emptyBox.length;
	if(num_of_tr <= getem){
		pageUl.style.display = 'none';
	}else{
		pageUl.style.display = 'flex';
		const num_Of_Page = Math.ceil(num_of_tr/getem);
		for(i=1; i<=num_Of_Page; i++){
			const li = document.createElement('li'); li.className = 'list';
			const a =document.createElement('a'); a.href = '#'; a.innerText = i;
			a.setAttribute('data-page', i);
			li.appendChild(a);
			pageUl.insertBefore(li,pageUl.querySelector('.next'));
		}
	}
}
pageGenerator(itemPerPage);
let pageLink = pageUl.querySelectorAll("a");
let lastPage =  pageLink.length - 2;

function pageRunner(page, items, lastPage, active){
	for(button of page){
		button.onclick = e=>{
			const page_num = e.target.getAttribute('data-page');
			const page_mover = e.target.getAttribute('id');
			if(page_num != null){
				index = page_num;

			}else{
				if(page_mover === "next"){
					index++;
					if(index >= lastPage){
						index = lastPage;
					}
				}else{
					index--;
					if(index <= 1){
						index = 1;
					}
				}
			}
			pageMaker(index, items, active);
		}
	}

}
var pageLi = pageUl.querySelectorAll('.list'); pageLi[0].classList.add("active");
pageRunner(pageLink, itemPerPage, lastPage, pageLi);

function getpagElement(val){
	let pagelink = pageUl.querySelectorAll("a");
	let lastpage =  pagelink.length - 2;
	let pageli = pageUl.querySelectorAll('.list');
	pageli[0].classList.add("active");
	pageRunner(pagelink, val, lastpage, pageli);
	
}



function pageMaker(index, item_per_page, activePage){
	const start = item_per_page * index;
	const end  = start + item_per_page;
	const current_page =  emptyBox.slice((start - item_per_page), (end-item_per_page));
	tbody.innerHTML = "";
	for(let j=0; j<current_page.length; j++){
		let item = current_page[j];					
		tbody.appendChild(item);
	}
	Array.from(activePage).forEach((e)=>{e.classList.remove("active");});
	 activePage[index-1].classList.add("active");
};


