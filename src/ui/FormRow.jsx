export default function FormRow({ label, error, children }) {
  return (
    <div className='flex flex-col sm:grid sm:grid-cols-4 sm:items-start gap-2 sm:gap-4 w-full pb-2'>
      <label className='text-[15px] font-medium text-[var(--primary)] capitalize pt-2 sm:col-span-1'>
        {label}
      </label>
      <div className='w-full sm:col-span-3 flex flex-col gap-1'>
        {children}
        {/* عرض رسالة الخطأ هنا */}
        {error && <span className="text-red-500 text-sm font-medium mt-1">{error}</span>}
      </div>
    </div>  
  )
}