export default function Input({
  type,
  placeholder,
  id,
  min,
  max,
  register,
  name,
  validation = {}
}) {
  return (
    <input
      className="
      w-full
      border
      border-gray-300
      rounded-xl
      px-4
      py-2
      focus:ring-2
      focus:ring-[var(--secondary)]/20
      outline-none
      "
      type={type}
      placeholder={placeholder}
      id={id}
      min={min}
      max={max}
      {...register(name, validation)}
    />
  );
}