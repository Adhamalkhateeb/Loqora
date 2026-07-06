import React from 'react';

export default function Form({ children, onSubmit }) {
  return (
    <form onSubmit={onSubmit} className='w-full max-w-2xl mx-auto my-6 p-6 bg-[var(--white)] rounded-2xl shadow-sm border border-gray-100 
    flex flex-col gap-5 justify-start items-stretch'>
      {children}
    </form>
  );
}