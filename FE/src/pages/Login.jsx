import { Link } from "react-router-dom";
import { useForm } from "react-hook-form";
import Input from "../ui/Input";
import Form from "../ui/Form";
import FormRow from "../ui/FormRow";

export default function Login() {
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm();

  function onSubmit(data) {
    console.log(data);

    // login(data)
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-[var(--main)] px-5">

      <div className="w-full max-w-lg">

        <div className="mb-8 text-center">
          <h1 className="text-4xl font-bold text-[var(--primary)]">
            StayEase
          </h1>

          <p className="text-gray-500 mt-2">
            Welcome back 👋
          </p>
        </div>

        <Form onSubmit={handleSubmit(onSubmit)}>

          <FormRow
            label="Email"
            error={errors?.email?.message}
          >
            <Input
              type="email"
              placeholder="example@gmail.com"
              register={register}
              name="email"
              required={false}
              id="email"
              validation={{
                required:"Email is required",
                pattern:{
                    value:/^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                    message:"Invalid email"
                }
            }}
            />

          </FormRow>

          <FormRow
            label="Password"
            error={errors?.password?.message}
          >
            <Input
              type="password"
              placeholder="********"
              register={register}
              name="password"
              id="password"
              required={false}
            />

          </FormRow>

          <div className="flex justify-between items-center">

            <label className="flex gap-2 items-center cursor-pointer">

              <input
                type="checkbox"
                {...register("remember")}
              />

              <span className="text-sm">
                Remember me
              </span>

            </label>

            <Link
              to="/forgot-password"
              className="text-sm text-blue-600 hover:underline"
            >
              Forgot Password?
            </Link>

          </div>

          <button
            disabled={isSubmitting}
            className="
            bg-[var(--secondary)]
            hover:opacity-90
            text-white
            rounded-xl
            py-3
            font-semibold
            transition
            disabled:opacity-60
            "
          >
            {isSubmitting ? "Signing In..." : "Sign In"}
          </button>

          <div className="text-center">

            <span className="text-gray-500">
              Don't have an account?
            </span>

            <Link
              to="/register"
              className="text-blue-600 ml-2 hover:underline"
            >
              Register
            </Link>

          </div>

        </Form>

      </div>

    </div>
  );
}