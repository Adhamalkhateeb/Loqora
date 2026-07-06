import { useMemo } from "react";
import { Link } from "react-router-dom";
import { useForm } from "react-hook-form";
import Input from "../ui/Input";
import FormRow from "../ui/FormRow";
import Form from "../ui/Form";


const PASSWORD_REGEX =
  /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?#&^()_\-+=])[A-Za-z\d@$!%*?#&^()_\-+=]{6,30}$/;

export default function Register() {
  const {
    register,
    handleSubmit,
    watch,
    formState: { errors, isSubmitting },
  } = useForm();

  const password = watch("password", "");

  const passwordChecks = useMemo(() => {
    return {
      length: password.length >= 6 && password.length <= 30,
      upper: /[A-Z]/.test(password),
      lower: /[a-z]/.test(password),
      number: /\d/.test(password),
      special: /[@$!%*?#&^()_\-+=]/.test(password),
    };
  }, [password]);

  const score = Object.values(passwordChecks).filter(Boolean).length;

  function onSubmit(data) {
    console.log(data);

    // registerUser(data)
  }

  return (
    <div className="min-h-screen flex justify-center items-center bg-[var(--main)] px-5 py-10">
      <div className="w-full max-w-3xl">
        <div className="text-center mb-8">
          <h1 className="text-4xl font-bold text-[var(--primary)]">
            Create Account
          </h1>

          <p className="text-gray-500 mt-2">
            Welcome to StayEase Dashboard
          </p>
        </div>

        <Form onSubmit={handleSubmit(onSubmit)}>
          <div className="grid md:grid-cols-2 gap-4">
            <FormRow
              label="First Name"
              error={errors.firstName?.message}
            >
              <Input
                type="text"
                name="firstName"
                placeholder="Ahmed"
                register={register}
                validation={{
                  required: "First name is required",
                }}
              />
            </FormRow>

            <FormRow
              label="Last Name"
              error={errors.lastName?.message}
            >
              <Input
                type="text"
                name="lastName"
                placeholder="Elmansy"
                register={register}
                validation={{
                  required: "Last name is required",
                }}
              />
            </FormRow>
          </div>

          <FormRow
            label="Email"
            error={errors.email?.message}
          >
            <Input
              type="email"
              name="email"
              placeholder="ahmed@gmail.com"
              register={register}
              validation={{
                required: "Email is required",
                pattern: {
                  value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                  message: "Invalid email",
                },
              }}
            />
          </FormRow>

          <FormRow
            label="Confirm Email"
            error={errors.confirmEmail?.message}
          >
            <Input
              type="email"
              name="confirmEmail"
              placeholder="Confirm email"
              register={register}
              validation={{
                required: "Confirm your email",
                validate: (value) =>
                  value === watch("email") || "Emails don't match",
              }}
            />
          </FormRow>

          <FormRow
            label="Phone"
            error={errors.phone?.message}
          >
            <Input
              type="text"
              name="phone"
              placeholder="+201012345678"
              register={register}
              validation={{
                required: "Phone is required",
                pattern: {
                  value: /^(\+20|0)?1[0125][0-9]{8}$/,
                  message: "Invalid Egyptian phone",
                },
              }}
            />
          </FormRow>

          <FormRow
            label="Password"
            error={errors.password?.message}
          >
            <Input
              type="password"
              name="password"
              placeholder="********"
              register={register}
              validation={{
                required: "Password is required",
                pattern: {
                  value: PASSWORD_REGEX,
                  message:
                    "6-30 chars, uppercase, lowercase, number & special character",
                },
              }}
            />

            <div className="w-full h-2 rounded-full bg-gray-200 mt-3 overflow-hidden">
              <div
                className={`h-full transition-all ${
                  score <= 2
                    ? "bg-red-500 w-1/3"
                    : score <= 4
                    ? "bg-yellow-500 w-2/3"
                    : "bg-green-500 w-full"
                }`}
              />
            </div>

            {/* <div className="mt-3 text-sm space-y-1">

              <p className={passwordChecks.length ? "text-green-600" : "text-gray-500"}>
                ✓ 6 - 30 characters
              </p>

              <p className={passwordChecks.upper ? "text-green-600" : "text-gray-500"}>
                ✓ Uppercase Letter
              </p>

              <p className={passwordChecks.lower ? "text-green-600" : "text-gray-500"}>
                ✓ Lowercase Letter
              </p>

              <p className={passwordChecks.number ? "text-green-600" : "text-gray-500"}>
                ✓ Number
              </p>

              <p className={passwordChecks.special ? "text-green-600" : "text-gray-500"}>
                ✓ Special Character
              </p>
            </div> */}
          </FormRow>

          <FormRow
            label="Confirm Password"
            error={errors.confirmPassword?.message}
          >
            <Input
              type="password"
              name="confirmPassword"
              placeholder="********"
              register={register}
              validation={{
                required: "Confirm password",
                validate: (value) =>
                  value === watch("password") ||
                  "Passwords don't match",
              }}
            />
          </FormRow>

          <label className="flex items-center gap-2">
            <input
              type="checkbox"
              {...register("agree", {
                required: "Accept terms first",
              })}
            />

            <span>
              I agree to Terms & Conditions
            </span>
          </label>

          {errors.agree && (
            <p className="text-red-500 text-sm">
              {errors.agree.message}
            </p>
          )}

          <button
            disabled={isSubmitting}
            className="
            bg-[var(--secondary)]
            text-white
            rounded-xl
            py-3
            font-semibold
            hover:opacity-90
            transition
            "
          >
            {isSubmitting
              ? "Creating..."
              : "Create Account"}
          </button>

          <div className="text-center">
            Already have an account?

            <Link
              className="ml-2 text-blue-600 hover:underline"
              to="/login"
            >
              Sign In
            </Link>
          </div>
        </Form>
      </div>
    </div>
  );
}