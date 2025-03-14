import { Label } from "flowbite-react";
import React from "react";
import DatePicker, { DatePickerProps } from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import { useController, UseControllerProps } from "react-hook-form";

type Props = {
    label: string;
    type?: string;
    showLabel?: boolean;
} & UseControllerProps &
    DatePickerProps;

export default function DateInput(props: Props) {
    //fiedstate is where the errors, etc are located
    const { fieldState, field } = useController({ ...props, defaultValue: "" });
    return (
        <div className="mb-3">
            {props.showLabel && (
                <div className="mb-2 block">
                    <Label htmlFor={field.name} value={props.label} />
                </div>
            )}
            <DatePicker
                {...props}
                {...field}
                //type can be number/ date but default to text
                placeholderText={props.label}
                //dynamic colors based on validation
                selected={field.value}
                className={`rounded-lg w-[100%] flex flex-col
                    ${
                        fieldState.error
                            ? "bg-red-50 border-red-500 text-red-900"
                            : !fieldState.invalid && fieldState.isDirty
                            ? "bg-green-50 border-green-500 text-green-900"
                            : ""
                    }`}
            />
            {fieldState.error && (
                <div className="text-red-500 text-sm ">
                    {fieldState.error.message}
                </div>
            )}
        </div>
    );
}
