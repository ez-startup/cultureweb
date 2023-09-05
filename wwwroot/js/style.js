
$(document).ready(function () {
	var owl = $('.carousel1');
	owl.owlCarousel({
		items: 10,
		loop: true,
		margin: 5,
		autoplay: true,
		slideTransition: 'linear',
		autoplayTimeout: 5000,
		autoplaySpeed: 5000,
		autoplayHoverPause: false,
		nav: false,
		dots: false,
		responsive: {
			0: {
				items: 2,
				margin: 5,

			},
			400: {
				items: 3,
				margin: 5,

			},
			600: {
				items: 4,
				margin: 5,

			},
			800: {
				items: 5,
				margin: 5,

			},
			1000: {
				items: 5,
				margin: 5,

			},
			1200: {
				items: 6,
				margin: 5,

			},
			1400: {
				items: 7,
				margin: 5,

			},
			1500: {
				items: 8,
				margin: 5,

			},
		}

	});

});

$(document).ready(function () {
  var owl2 = $('.carousel2');
	owl2.owlCarousel({
    items: 5,
    loop: true,
	margin: 15,
	autoplay: true,
    slideTransition: 'linear',
    autoplayTimeout: 5000,
    autoplaySpeed: 1000,
    autoplayHoverPause: false,
    nav:false,
    dots: false,
	  responsive: {
		  0: {
			  items: 1,
		  },
		  400: {
			  items: 1,
		  },
		  600: {
			  items: 2,			 
		  },
		  800: {
			  items: 3,			  
		  },
		  1000: {
			  items: 3,			 
		  },
		  1200: {
			  items: 4,	
			  
		  },
		  1400: {
			  items: 5,	
			 
		  },
		  1500: {
			  items: 5,
			
		  },
	  }
  });
});

//To the top
// Get the button
let toTop = document.getElementById("toTop");

// When the user scrolls down 20px from the top of the document, show the button
window.onscroll = function() {scrollFunction()};

function scrollFunction() {
  if (document.body.scrollTop > 20 || document.documentElement.scrollTop > 20) {
	  toTop.style.display = "block";
  } else {
	  toTop.style.display = "none";
  }
}

// When the user clicks on the button, scroll to the top of the document
function topFunction() {
  document.body.scrollTop = 0;
  document.documentElement.scrollTop = 0;
}

//Star Reviews
var stars = [
	document.getElementById("star1"),
	document.getElementById("star2"),
	document.getElementById("star3"),
	document.getElementById("star4"),
	document.getElementById("star5")
];

for (var i = 0; i < stars.length; i++) {
	stars[i].addEventListener("click", createStarClickHandler(i));
}

function createStarClickHandler(index) {
	return function () {
		for (var j = 0; j <= index; j++) {
			stars[j].style.color = "red";
		}
		for (var j = index + 1; j < stars.length; j++) {
			stars[j].style.color = "";
		}
	};
}

  //pagination
  var tbody = document.querySelector(".category-body");
		var pageUl = document.querySelector(".pagination");
		var itemShow = document.querySelector("#itemperpage");
		var tr = tbody.querySelectorAll(".row");
		var emptyBox = [];
		var index = 1;
		var itemPerPage = 15;

		for(let i=0; i<tr.length; i++){ emptyBox.push(tr[i]);}

		itemShow.onchange = giveTrPerPage;
		function giveTrPerPage(){
			itemPerPage = Number(this.value)
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
		}

		//product-review
		document.addEventListener('DOMContentLoaded', function () {
			const starRadios = document.querySelectorAll('.star-radio');

			starRadios.forEach(radio => {
				radio.addEventListener('click', function () {
					const value = parseInt(this.value);
					for (let i = 1; i <= 5; i++) {
						const starIcon = document.querySelector(`#rating-${i} + .rating-star i`);
						starIcon.style.color = i <= value ? 'red' : '#ccc';
					}
				});
			});
		});


		


