import { NavLink } from "react-router-dom";

export default function IconSideBar({icon , text ,link}) {
  return (
        <li className="mt-2 " >
         <NavLink to={link} 
         className="
         flex items-center  gap-1 px-2 py-1 rounded-2xl

         hover:text-blue-600 hover:bg-white   " >
         
          <span className="text-xl">{icon}</span>
          <h1 className="text-[15px]">{text}</h1>
        
        </NavLink>
        </li>
  )
}
