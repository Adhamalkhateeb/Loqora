import { FaSearch } from "react-icons/fa";

export default function Search({text}) {
  return (
    <div className=' flex items-center bg-[var(--main)] p-2 rounded-xl w-fit gap-3'> 
    <span className="text-[16px]"><FaSearch/></span>
    <input 
      className='border-orange-50 outline-none w-60 text-[9px]'
      type="search" name="search" id="search" placeholder={text} />
    </div> 

  )
}
