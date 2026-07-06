import { useState } from "react";
import { IoMdMenu } from "react-icons/io";
import SideBar from "./SideBar";
import { IoIosCloseCircle } from "react-icons/io";

export default function Header() {
  const [isOpen ,setIsOpen] =useState(false)
  return (
    <div className=' h-10 text-center relative flex items-center border-b-1'>
   <div>
      <div className="text-2xl lg:hidden cursor-pointer" onClick={() => setIsOpen(!isOpen)}>
        <IoMdMenu />
      </div>

      <div className={`fixed inset-0 z-50 transition-all duration-300 ${
        isOpen ? "visible pointer-events-auto" : "invisible pointer-events-none"
      }`}>
        
        <div 
          className={`absolute inset-0 bg-black/40 transition-opacity duration-1000 ${
            isOpen ? "opacity-100" : "opacity-0"
          }`}
          onClick={() => setIsOpen(false)}
        />

        <div className={` lg:hidden absolute top-0 left-0 h-full transition-transform duration-1000 ease-in-out ${
          isOpen ? "translate-x-0" : "-translate-x-full"
        }`}>
          
          <div 
            className="absolute right-3 top-3 text-xl cursor-pointer text-gray-500 hover:text-gray-800 z-50" 
            onClick={() => setIsOpen(false)}
          >
            <IoIosCloseCircle />
          </div>

          <SideBar />
        </div>
      </div>
   </div>
   
   
     <div>

     </div>
    </div>
  )
}
