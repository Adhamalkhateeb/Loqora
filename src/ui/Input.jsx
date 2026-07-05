export default function Input({ type, placeholder, id, min, max, register, name, required }) {
  return (
    <input  
      className='w-full border border-gray-350 rounded-xl px-4 py-2 text-gray-700 placeholder-gray-400 focus:outline-none focus:border-[var(--secondary)] focus:ring-2 focus:ring-[var(--secondary)]/10 transition-all duration-200'
      type={type} 
      placeholder={placeholder} 
      id={id} 
      min={min} 
      max={max}
      {...register(name, { required: required && "This field is required" })}
    />
  )
}